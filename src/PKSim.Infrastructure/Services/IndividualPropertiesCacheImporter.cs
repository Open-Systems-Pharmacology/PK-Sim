using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LumenWorks.Framework.IO.Csv;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Extensions;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

//TODO 
namespace PKSim.Infrastructure.Services
{
   public class IndividualPropertiesCacheImporter : IIndividualPropertiesCacheImporter
   {
      private readonly IGenderRepository _genderRepository;
      private readonly IPopulationRepository _populationRepository;
      private readonly IIndividualValuesCacheImporter _individualValuesCacheImporter;

      public IndividualPropertiesCacheImporter(IGenderRepository genderRepository, IPopulationRepository populationRepository, IIndividualValuesCacheImporter individualValuesCacheImporter)
      {
         _genderRepository = genderRepository;
         _populationRepository = populationRepository;
         _individualValuesCacheImporter = individualValuesCacheImporter;
      }


      public IndividualValuesCache ImportFrom(string fileFullPath, PathCache<IParameter> allParameters, IImportLogger logger)
      {

         var individualValuesCache = _individualValuesCacheImporter.ImportFrom(fileFullPath, logger, allParameters);

         return withUpdatedGenderAndRace(individualValuesCache);
      }

      private IndividualValuesCache withUpdatedGenderAndRace(IndividualValuesCache individualValuesCache)
      {
         var parameterValues = individualValuesCache.ParameterValuesCache;
         var covariateValuesCache = new CovariateValuesCache();

         individualValuesCache.CovariateValuesCache.AllCovariateValues.Each(x =>
         {
            if (!x.NameIsOneOf(Constants.Population.RACE_INDEX, Constants.Population.GENDER))
               covariateValuesCache.Add(x);

            else if(x.IsNamed(Constants.Population.RACE_INDEX))
               covariateValuesCache.Add(populationCovariateFrom(x));

            else
               covariateValuesCache.Add(genderCovariateFrom(x));
         });

         return new IndividualValuesCache(parameterValues, covariateValuesCache);
      }

      private CovariateValues genderCovariateFrom(CovariateValues originalCovariateValues)
      {
         if(originalCovariateValues.Count==0)
            return new CovariateValues(Constants.Population.GENDER);

         var firstValue = originalCovariateValues.ValueAt(0);

         //Not a number => Already to the newest format
         if (!int.TryParse(firstValue, out _))
            return originalCovariateValues;

         // needs conversion
         var genders = originalCovariateValues.Values.Select(x => _genderRepository.FindByIndex(int.Parse(x))).Select(x => x.Name).ToList();
         return new CovariateValues(Constants.Population.GENDER, genders);
      }
      
      private CovariateValues populationCovariateFrom(CovariateValues originalCovariateValues)
      {
         if (originalCovariateValues.Count == 0)
            return new CovariateValues(Constants.Population.POPULATION);

         //Not a number => Already to the newest format
         var firstValue = originalCovariateValues.ValueAt(0);
         if (!int.TryParse(firstValue, out _))
            return originalCovariateValues;

         // needs conversion
         var races = originalCovariateValues.Values.Select(x => _populationRepository.FindByIndex(int.Parse(x))).Select(x => x.Name).ToList();
         return new CovariateValues(Constants.Population.POPULATION, races);
      }
   }
}