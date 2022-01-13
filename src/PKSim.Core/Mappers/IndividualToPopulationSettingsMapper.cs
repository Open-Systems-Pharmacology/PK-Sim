using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Parameters;

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

         var population = individual.OriginData.Population;
         populationSettings.BaseIndividual = individual;
         populationSettings.NumberOfIndividuals = CoreConstants.DEFAULT_NUMBER_OF_INDIVIDUALS_IN_POPULATION;

         int genderCount = individual.AvailableGenders.Count;
         foreach (var gender in individual.AvailableGenders)
         {
            populationSettings.AddGenderRatio(new GenderRatio {Gender = gender, Ratio = 100 / genderCount});
         }

         var organism = individual.Organism;
         if (individual.IsAgeDependent)
            populationSettings.AddParameterRange(constrainedParameterRangeFrom(organism.Parameter(AGE)));

         if (individual.IsPreterm)
         {
            var gestationalAgeParameter = organism.Parameter(GESTATIONAL_AGE);
            populationSettings.AddParameterRange(discreteParameterRangeFrom(gestationalAgeParameter, numericListOfValues(gestationalAgeParameter)));
         }

         if (population.IsHeightDependent)
            populationSettings.AddParameterRange(parameterRangeFrom(organism.Parameter(MEAN_HEIGHT)));

         var weightParameter = organism.Parameter(MEAN_WEIGHT);
         populationSettings.AddParameterRange(population.IsAgeDependent ? parameterRangeFrom(weightParameter) : constrainedParameterRangeFrom(weightParameter));

         if (population.IsHeightDependent)
            populationSettings.AddParameterRange(parameterRangeFrom(organism.Parameter(BMI)));

         individual.OriginData.DiseaseStateParameters.Each(x =>
         {
            var parameter = individual.OriginData.DiseaseState.Parameter(x.Name);
            populationSettings.AddParameterRange(constrainedParameterRangeFrom(parameter));
         });
         return populationSettings;
      }

      private ParameterRange constrainedParameterRangeFrom(IParameter parameter)
      {
         var parameterRange = createParameterRange<ConstrainedParameterRange>(parameter);
         parameterRange.MinValue = parameter.MinValue;
         parameterRange.MaxValue = parameter.MaxValue;
         return parameterRange;
      }

      private ParameterRange discreteParameterRangeFrom(IParameter parameter, IEnumerable<double> listOfValues)
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