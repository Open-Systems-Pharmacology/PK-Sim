using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using LumenWorks.Framework.IO.Csv;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

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

      private const int PARAMETER_PATH = 0;
      private readonly Char[] _allowedDelimiters = {';', '\t'};

      public IndividualPropertiesCache ImportFrom(string fileFullPath, IImportLogger logger)
      {
         try
         {
            foreach (var delimiter in _allowedDelimiters)
            {
               var individualPropertyCache = individualPropertiesCacheFrom(fileFullPath, logger, delimiter);
               //we found at least one individual, this is a valid file for the delimiter and we can exit
               if (individualPropertyCache.Count > 0)
                  return individualPropertyCache;
            }

            return new IndividualPropertiesCache();
         }
         catch (Exception e)
         {
            logger.AddError(e.FullMessage());
            return new IndividualPropertiesCache();
         }
      }

      private IndividualPropertiesCache individualPropertiesCacheFrom(string fileFullPath, IImportLogger logger, char delimiter)
      {
         var individualPropertiesCache = new IndividualPropertiesCache();

         using (var reader = new CsvReaderDisposer(fileFullPath, delimiter))
         {
            var csv = reader.Csv;
            var headers = csv.GetFieldHeaders();
            if (headers.Contains(Constants.Population.INDIVIDUAL_ID_COLUMN))
               loadNewCSVFormat(individualPropertiesCache, csv, headers);
            else
            {
               loadOldCSVFormat(individualPropertiesCache, csv);
               logger.AddWarning(PKSimConstants.Warning.PopulationFileIsUsingOldFormatAndWontBeSupportedInTheNextVersion);
            }
         }

         return individualPropertiesCache;
      }

      private void loadNewCSVFormat(IndividualPropertiesCache individualPropertiesCache, CsvReader csv, string[] headers)
      {
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
      }

      private void addCovariates(IndividualPropertiesCache individualPropertiesCache, Cache<string, List<string>> covariateCache)
      {
         foreach (var covariate in covariateCache.KeyValues)
         {
            if (covariate.Key.IsOneOf(CoreConstants.Parameter.RACE_INDEX, CoreConstants.Parameter.GENDER))
               addCovariates(individualPropertiesCache, covariate.Key, covariate.Value.Select(x => double.Parse(x, NumberFormatInfo.InvariantInfo)));
            else
               individualPropertiesCache.AddConvariate(covariate.Key, covariate.Value);
         }
      }

      private void loadOldCSVFormat(IndividualPropertiesCache individualPropertiesCache, CsvReader csv)
      {
         int fieldCount = csv.FieldCount;

         while (csv.ReadNextRecord())
         {
            var parameterPath = csv[PARAMETER_PATH];
            var values = getValuesFrom(csv, fieldCount);

            if (entryRepresentsParameter(parameterPath))
            {
               ensureCovariatesAreDefined(individualPropertiesCache);
               individualPropertiesCache.SetValues(parameterPath, values);
            }
            else
               addCovariates(individualPropertiesCache, parameterPath, values);
         }
      }

      private void ensureCovariatesAreDefined(IndividualPropertiesCache individualPropertiesCache)
      {
         //this should ensure that the covariates were defined for the population. If not, the file does not have the accurate structure
         if (!individualPropertiesCache.AllCovariates.Any())
            throw new PKSimException(PKSimConstants.Error.GenderAndOrPopulationMissingFromFile);
      }

      private void addCovariates(IndividualPropertiesCache individualPropertiesCache, string parameterPath, IEnumerable<double> values)
      {
         if (string.Equals(parameterPath, CoreConstants.Parameter.RACE_INDEX))
            individualPropertiesCache.AddPopulations(values.Select(index => _populationRepository.FindByIndex((int) index)).ToList());

         else if (string.Equals(parameterPath, CoreConstants.Parameter.GENDER))
            individualPropertiesCache.AddGenders(values.Select(index => _genderRepository.FindByIndex((int) index)).ToList());
      }

      private bool entryRepresentsParameter(string parameterPath)
      {
         if (parameterPath.IsOneOf(CoreConstants.Parameter.RACE_INDEX, CoreConstants.Parameter.GENDER))
            return false;

         return parameterPath.Contains(ObjectPath.PATH_DELIMITER);
      }

      private IEnumerable<double> getValuesFrom(CsvReader csv, int fieldCount)
      {
         var values = new List<double>();
         for (int i = PARAMETER_PATH + 1; i < fieldCount; i++)
         {
            values.Add(csv.DoubleAt(i));
         }
         return values;
      }
   }
}