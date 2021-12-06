using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Core.Services
{
   public interface IDistributionDataCreator
   {
      ContinuousDistributionData CreateFor(IVectorialParametersContainer parameterContainer, ParameterDistributionSettings distributionSettings);
      ContinuousDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, IParameter parameter, IDimension dimension, Unit displayUnit);
      ContinuousDistributionData CreateFor(IPopulationDataCollector populationDataCollector, DistributionSettings settings, QuantityPKParameter parameter, IDimension dimension, Unit displayUnit);
      DiscreteDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, string covariate);
      DiscreteDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, IReadOnlyList<string> allDiscreteValues, IComparer<string> comparer);
   }

   public class DistributionDataCreator : IDistributionDataCreator
   {
      private readonly IBinIntervalsCreator _binIntervalsCreator;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ICoreUserSettings _userSettings;
      private readonly IGenderRepository _genderRepository;

      public DistributionDataCreator(
         IBinIntervalsCreator binIntervalsCreator, 
         IRepresentationInfoRepository representationInfoRepository, 
         IEntityPathResolver entityPathResolver, 
         ICoreUserSettings userSettings,
         IGenderRepository genderRepository)
      {
         _binIntervalsCreator = binIntervalsCreator;
         _representationInfoRepository = representationInfoRepository;
         _entityPathResolver = entityPathResolver;
         _userSettings = userSettings;
         _genderRepository = genderRepository;
      }

      public ContinuousDistributionData CreateFor(IPopulationDataCollector populationDataCollector, DistributionSettings settings, QuantityPKParameter parameter, IDimension dimension, Unit displayUnit)
      {
         var allValues = populationDataCollector.AllPKParameterValuesFor(parameter.QuantityPath, parameter.Name);
         return CreateFor(populationDataCollector, settings, allValues, dimension, displayUnit);
      }

      public ContinuousDistributionData CreateFor(IVectorialParametersContainer parameterContainer, ParameterDistributionSettings distributionSettings)
      {
         var parameter = parameterContainer.ParameterByPath(distributionSettings.ParameterPath, _entityPathResolver);
         if (parameter == null)
            return new ContinuousDistributionData(AxisCountMode.Count, _userSettings.NumberOfBins);

         return CreateFor(parameterContainer, distributionSettings.Settings, parameter, parameter.Dimension, parameter.DisplayUnit);
      }

      public ContinuousDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, IParameter parameter, IDimension dimension, Unit displayUnit)
      {
         var allValues = parameterContainer.AllValuesFor(_entityPathResolver.PathFor(parameter));
         return CreateFor(parameterContainer, settings, allValues, dimension, displayUnit);
      }

      private  IReadOnlyList<Gender> allGendersFrom(IVectorialParametersContainer parameterContainer, int count)
      {
         var allGenders = parameterContainer.AllGenders(_genderRepository);
         if (allGenders.Count == count)
            return allGenders;

         var unknownGender = new Gender {Name = CoreConstants.Gender.UNDEFINED};
         return new Gender[count].InitializeWith(unknownGender);
      }

      public DiscreteDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, string covariate)
      {
         var allValues = parameterContainer.AllCovariateValuesFor(covariate);
         return CreateFor(parameterContainer, settings, allValues, Comparer<string>.Default);
      }

      public ContinuousDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, IReadOnlyList<double> values, IDimension dimension, Unit displayUnit)
      {
         var allValuesInDisplayUnit = values.Select(value => dimension.BaseUnitValueToUnitValue(displayUnit, value)).ToList();
         var allGenders = allGendersFrom(parameterContainer, allValuesInDisplayUnit.Count);
         var allValidValues = allValuesInDisplayUnit.Where(x => !double.IsNaN(x)).ToList();

         var allIntervals = _binIntervalsCreator.CreateUniformIntervalsFor(allValidValues);

         var data = new ContinuousDistributionData(settings.AxisCountMode, allIntervals.Count);

         if (!allValidValues.Any())
            return data;

         int allItemsForSelectedGender = numberOfItemsFor(allGenders, settings.SelectedGender);

         //group the values by gender
         var valueGenders = allValuesInDisplayUnit.Select((value, index) => new {Value = value, Gender = allGenders[index]})
            .Where(x => !double.IsNaN(x.Value))
            .GroupBy(x => x.Gender)
            .ToList();


         foreach (var interval in allIntervals)
         {
            var currentInterval = interval;
            foreach (var valueGender in valueGenders)
            {
               if (!shouldDisplayGender(valueGender.Key, settings.SelectedGender)) continue;

               double count = valueGender.Count(item => currentInterval.Contains(item.Value));
               if (settings.AxisCountMode == AxisCountMode.Percent)
                  count = count * 100.0 / allItemsForSelectedGender;

               data.AddData(currentInterval.MeanValue, count, _representationInfoRepository.DisplayNameFor(valueGender.Key));
            }
         }

         data.XMinData = allValidValues.Min();
         data.XMaxData = allValidValues.Max();

         return data;
      }

      public DiscreteDistributionData CreateFor(IVectorialParametersContainer parameterContainer, DistributionSettings settings, IReadOnlyList<string> allDiscreteValues, IComparer<string> comparer)
      {
         var data = new DiscreteDistributionData(settings.AxisCountMode);

         //all genders
         var allGenders = allGendersFrom(parameterContainer, allDiscreteValues.Count);
         int allItemsForSelectedGender = numberOfItemsFor(allGenders, settings.SelectedGender);

         //group the values by gender
         var valueGenders = allDiscreteValues.Select((value, index) => new {Value = value, Gender = allGenders[index]})
            .OrderBy(x => x.Value, comparer)
            .GroupBy(x => x.Gender);

         foreach (var valueGender in valueGenders)
         {
            if (!shouldDisplayGender(valueGender.Key, settings.SelectedGender))
               continue;

            var valuesByGenderAndCovariates = valueGender.GroupBy(x => x.Value).ToList();
            foreach (var value in valuesByGenderAndCovariates)
            {
               double count = value.Count();
               if (settings.AxisCountMode == AxisCountMode.Percent)
                  count = count * 100.0 / allItemsForSelectedGender;

               data.AddData(value.Key, count, _representationInfoRepository.DisplayNameFor(valueGender.Key));
            }
         }
         return data;
      }

      private int numberOfItemsFor(IReadOnlyList<Gender> allGenders, string selectedGender)
      {
         if (string.Equals(selectedGender, Constants.Population.ALL_GENDERS))
            return allGenders.Count;

         return allGenders.Count(x => string.Equals(x.Name, selectedGender));
      }

      private bool shouldDisplayGender(Gender gender, string selectedGender)
      {
         return string.Equals(selectedGender, Constants.Population.ALL_GENDERS) ||
                string.Equals(gender.Name, selectedGender);
      }
   }
}