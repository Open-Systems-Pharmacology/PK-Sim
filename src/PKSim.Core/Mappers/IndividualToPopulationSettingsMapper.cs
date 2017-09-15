using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToPopulationSettingsMapper : IMapper<Individual, RandomPopulationSettings>
   {
   }

   public class IndividualToPopulationSettingsMapper : IIndividualToPopulationSettingsMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public IndividualToPopulationSettingsMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public RandomPopulationSettings MapFrom(Individual individual)
      {
         var populationSettings = new RandomPopulationSettings();
         if (individual == null)
            return populationSettings;

         var population = individual.OriginData.SpeciesPopulation;
         populationSettings.BaseIndividual = individual;
         populationSettings.NumberOfIndividuals = CoreConstants.DEFAULT_NUMBER_OF_INDIVIDUALS_IN_POPULATION;

         int genderCount = individual.AvailableGenders().Count();
         foreach (var gender in individual.AvailableGenders())
         {
            populationSettings.AddGenderRatio(new GenderRatio {Gender = gender, Ratio = 100 / genderCount});
         }

         if (individual.IsAgeDependent)
         {
            var ageParameter = individual.Organism.Parameter(CoreConstants.Parameter.AGE);
            populationSettings.AddParameterRange(constrainedParameterRangeFrom(ageParameter));
         }

         if (individual.IsPreterm)
         {
            var gestationalAgeParameter = individual.Organism.Parameter(CoreConstants.Parameter.GESTATIONAL_AGE);
            populationSettings.AddParameterRange(discretedParameterRangeFrom(gestationalAgeParameter, numericListOfValues(gestationalAgeParameter)));
         }

         if (population.IsHeightDependent)
         {
            var heightParameter = individual.Organism.Parameter(CoreConstants.Parameter.MEAN_HEIGHT);
            populationSettings.AddParameterRange(parameterRangeFrom(heightParameter));
         }

         var weightParameter = individual.Organism.Parameter(CoreConstants.Parameter.MEAN_WEIGHT);

         if (population.IsAgeDependent)
            populationSettings.AddParameterRange(parameterRangeFrom(weightParameter));
         else
            populationSettings.AddParameterRange(constrainedParameterRangeFrom(weightParameter));

         if (population.IsHeightDependent)
         {
            var bmiParameter = individual.Organism.Parameter(CoreConstants.Parameter.BMI);
            populationSettings.AddParameterRange(parameterRangeFrom(bmiParameter));
         }
         return populationSettings;
      }

      private ParameterRange constrainedParameterRangeFrom(IParameter parameter)
      {
         var parameterRange = createParameterRange<ConstrainedParameterRange>(parameter);
         parameterRange.MinValue = parameter.MinValue;
         parameterRange.MaxValue = parameter.MaxValue;
         return parameterRange;
      }

      private ParameterRange discretedParameterRangeFrom(IParameter parameter, IEnumerable<double> listOfValues)
      {
         var parameterRange = createParameterRange<DiscreteParameterRange>(parameter);
         parameterRange.MinValue = parameter.MinValue;
         parameterRange.MaxValue = parameter.MaxValue;
         parameterRange.ListOfValues = listOfValues;
         return parameterRange;
      }

      private ParameterRange parameterRangeFrom(IParameter parameter) => createParameterRange<ParameterRange>(parameter);

      private TParameterRangeDTO createParameterRange<TParameterRangeDTO>(IParameter parameter) where TParameterRangeDTO : ParameterRange, new()
      {
         return new TParameterRangeDTO
         {
            ParameterName = parameter.Name,
            ParameterDisplayName = _representationInfoRepository.DisplayNameFor(parameter),
            Dimension = parameter.Dimension,
            Unit = parameter.DisplayUnit,
            DbMaxValue = parameter.MaxValue,
            DbMinValue = parameter.MinValue
         };
      }

      private IEnumerable<double> numericListOfValues(IParameter parameter)
      {
         return numericListOfValues(parameter.MinValue.Value.ConvertedTo<int>(), parameter.MaxValue.Value.ConvertedTo<int>());
      }

      private IEnumerable<double> numericListOfValues(int min, int max)
      {
         for (int i = min; i <= max; i++)
         {
            yield return i;
         }
      }
   }
}