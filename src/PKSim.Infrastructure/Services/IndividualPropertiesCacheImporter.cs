using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

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
            if (!x.CovariateName.IsOneOf(Constants.Population.RACE_INDEX, Constants.Population.GENDER))
               covariateValuesCache.Add(x);

            else if (string.Equals(x.CovariateName, Constants.Population.RACE_INDEX))
               covariateValuesCache.Add(populationCovariateFrom(x));

            else
               covariateValuesCache.Add(genderCovariateFrom(x));
         });

         return new IndividualValuesCache(parameterValues, covariateValuesCache, individualValuesCache.IndividualIds);
      }

      private CovariateValues genderCovariateFrom(CovariateValues originalCovariateValues)
      {
         if (originalCovariateValues.Count == 0)
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