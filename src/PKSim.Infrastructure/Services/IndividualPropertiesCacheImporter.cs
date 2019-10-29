using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LumenWorks.Framework.IO.Csv;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Extensions;

namespace PKSim.Infrastructure.Services
{
   public class IndividualPropertiesCacheImporter : IIndividualPropertiesCacheImporter
   {
      private readonly IGenderRepository _genderRepository;
      private readonly IPopulationRepository _populationRepository;

      public IndividualPropertiesCacheImporter(IGenderRepository genderRepository, IPopulationRepository populationRepository)
      {
         _genderRepository = genderRepository;
         _populationRepository = populationRepository;
      }

      private static readonly char[] ALLOWED_DELIMITERS = {',', ';', '\t'};

      public IndividualPropertiesCache ImportFrom(string fileFullPath, PathCache<IParameter> allParameters, IImportLogger logger)
      {
         try
         {
            foreach (var delimiter in ALLOWED_DELIMITERS)
            {
               var individualPropertyCache = individualPropertiesCacheFrom(fileFullPath, delimiter);
               //we found at least one individual, this is a valid file for the delimiter and we can exit
               if (individualPropertyCache?.Count > 0)
                  return withPathsContainingUnitsUpdated(individualPropertyCache, allParameters);
            }

            //no match. Log 
            logger.AddError(PKSimConstants.Warning.PopulationFileFormatIsNotSupported);
            return new IndividualPropertiesCache();
         }
         catch (Exception e)
         {
            logger.AddError(e.FullMessage());
            return new IndividualPropertiesCache();
         }
      }

      private IndividualPropertiesCache withPathsContainingUnitsUpdated(IndividualPropertiesCache individualPropertiesCache, PathCache<IParameter> allParameters)
      {
         individualPropertiesCache.AllParameterValues.ToList().Each(parameterValue => { removeUnits(individualPropertiesCache, parameterValue, allParameters); });
         return individualPropertiesCache;
      }

      private void removeUnits(IndividualPropertiesCache individualValues, ParameterValues parameterValue, PathCache<IParameter> allParameters)
      {
         var parameterPath = parameterValue.ParameterPath;
         if (allParameters.Contains(parameterPath))
            return;

         var pathWithUnitsRemoved = importedPathWithUnitsRemoved(parameterPath);
         if (allParameters.Contains(pathWithUnitsRemoved))
         {
            individualValues.RenamePath(parameterPath, pathWithUnitsRemoved);
            parameterValue.ParameterPath = pathWithUnitsRemoved;
         }
      }

      private string importedPathWithUnitsRemoved(string path)
      {
         if (!path.TrimEnd().EndsWith("]")) return path;

         var indexOfUnitStart = path.LastIndexOf("[", StringComparison.Ordinal);

         return indexOfUnitStart == -1 ? path : path.Remove(indexOfUnitStart, path.Length - indexOfUnitStart).TrimEnd();
      }

      private IndividualPropertiesCache individualPropertiesCacheFrom(string fileFullPath, char delimiter)
      {
         using (var reader = new CsvReaderDisposer(fileFullPath, delimiter))
         {
            var csv = reader.Csv;
            var headers = csv.GetFieldHeaders();
            if (headers.Contains(Constants.Population.INDIVIDUAL_ID_COLUMN))
               return createIndividualPropertiesFromCSV(csv, headers);
         }

         return null;
      }

      private IndividualPropertiesCache createIndividualPropertiesFromCSV(CsvReader csv, string[] headers)
      {
         var individualPropertiesCache = new IndividualPropertiesCache();

         //first create a cache of all possible values
         var covariateCache = new Cache<string, List<string>>();
         var parameterValues = new Cache<string, List<double>>();
         int fieldCount = csv.FieldCount;
         int indexIndividualId = 0;

         for (int i = 0; i < headers.Length; i++)
         {
            var header = headers[i];
            if (string.Equals(header, Constants.Population.INDIVIDUAL_ID_COLUMN))
            {
               indexIndividualId = i;
               continue;
            }

            if (entryRepresentsParameter(header))
               parameterValues[header] = new List<double>();
            else
               covariateCache[header] = new List<string>();
         }

         while (csv.ReadNextRecord())
         {
            for (int i = 0; i < fieldCount; i++)
            {
               if (i == indexIndividualId)
                  continue;

               var header = headers[i];
               if (parameterValues.Contains(header))
                  parameterValues[header].Add(csv.DoubleAt(i));
               else
                  covariateCache[header].Add(csv[i]);
            }
         }

         //now fill the property cache
         addCovariates(individualPropertiesCache, covariateCache);

         foreach (var parameterValue in parameterValues.KeyValues)
         {
            individualPropertiesCache.SetValues(parameterValue.Key, parameterValue.Value);
         }

         return individualPropertiesCache;
      }

      private void addCovariates(IndividualPropertiesCache individualPropertiesCache, Cache<string, List<string>> covariateCache)
      {
         foreach (var covariate in covariateCache.KeyValues)
         {
            if (covariate.Key.IsOneOf(CoreConstants.Parameters.RACE_INDEX, CoreConstants.Parameters.GENDER))
               addCovariates(individualPropertiesCache, covariate.Key, covariate.Value.Select(x => double.Parse(x, NumberFormatInfo.InvariantInfo)));
            else
               individualPropertiesCache.AddConvariate(covariate.Key, covariate.Value);
         }
      }

      private void addCovariates(IndividualPropertiesCache individualPropertiesCache, string parameterPath, IEnumerable<double> values)
      {
         if (string.Equals(parameterPath, CoreConstants.Parameters.RACE_INDEX))
            individualPropertiesCache.AddPopulations(values.Select(index => _populationRepository.FindByIndex((int) index)).ToList());

         else if (string.Equals(parameterPath, CoreConstants.Parameters.GENDER))
            individualPropertiesCache.AddGenders(values.Select(index => _genderRepository.FindByIndex((int) index)).ToList());
      }

      private bool entryRepresentsParameter(string parameterPath)
      {
         if (parameterPath.IsOneOf(CoreConstants.Parameters.RACE_INDEX, CoreConstants.Parameters.GENDER))
            return false;

         return parameterPath.Contains(ObjectPath.PATH_DELIMITER);
      }
   }
}