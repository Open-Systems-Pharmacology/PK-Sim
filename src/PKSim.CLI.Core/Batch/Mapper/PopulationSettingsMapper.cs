using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   internal interface IPopulationSettingsMapper : IMapper<PopulationSettings, RandomPopulationSettings>
   {
   }

   internal class PopulationSettingsMapper : IPopulationSettingsMapper
   {
      private readonly IOriginDataMapper _originDataMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualToPopulationSettingsMapper _individualToPopulationSettingsMapper;
      private readonly IGenderRepository _genderRepository;

      public PopulationSettingsMapper(IOriginDataMapper originDataMapper, IIndividualFactory individualFactory,
         IIndividualToPopulationSettingsMapper individualToPopulationSettingsMapper, IGenderRepository genderRepository)
      {
         _originDataMapper = originDataMapper;
         _individualFactory = individualFactory;
         _individualToPopulationSettingsMapper = individualToPopulationSettingsMapper;
         _genderRepository = genderRepository;
      }

      public RandomPopulationSettings MapFrom(PopulationSettings batchPopulationSettings)
      {
         var originData = originDataFrom(batchPopulationSettings);

         var individual = _individualFactory.CreateStandardFor(originData);
         var populationSettings = _individualToPopulationSettingsMapper.MapFrom(individual);
         populationSettings.NumberOfIndividuals = batchPopulationSettings.NumberOfIndividuals;

         setRange(populationSettings, CoreConstants.Parameter.AGE, batchPopulationSettings.MinAge, batchPopulationSettings.MaxAge);
         setRange(populationSettings, CoreConstants.Parameter.MEAN_HEIGHT, batchPopulationSettings.MinHeight, batchPopulationSettings.MaxHeight);
         setRange(populationSettings, CoreConstants.Parameter.MEAN_WEIGHT, batchPopulationSettings.MinWeight, batchPopulationSettings.MaxWeight);
         setRange(populationSettings, CoreConstants.Parameter.BMI, batchPopulationSettings.MinBMI, batchPopulationSettings.MaxBMI);

         if(populationSettings.ContainsParameterRangeFor(CoreConstants.Parameter.GESTATIONAL_AGE))
            setRange(populationSettings, CoreConstants.Parameter.GESTATIONAL_AGE, batchPopulationSettings.MinGestationalAge, batchPopulationSettings.MaxGestationalAge);

         //in case of multiple gender, adjust the ration according to the feamales proportion
         if (individual.AvailableGenders().Count() > 1)
         {
            populationSettings.GenderRatio(_genderRepository.Female).Ratio = batchPopulationSettings.ProportionOfFemales;
            populationSettings.GenderRatio(_genderRepository.Male).Ratio = 100 - batchPopulationSettings.ProportionOfFemales;
         }

         return populationSettings;
      }

      private Model.OriginData originDataFrom(PopulationSettings batchPopulationSettings)
      {
         //create default individual based on given data
         var batchOriginData = new OriginData
         {
            Species = batchPopulationSettings.Species,
            Population = batchPopulationSettings.Population
         };

         batchPopulationSettings.AllCalculationMethods.KeyValues.Each(kv => batchOriginData.AddCalculationMethod(kv.Key, kv.Value));

         return _originDataMapper.MapFrom(batchOriginData);
      }

      private void setRange(RandomPopulationSettings populationSettings, string parameterName, double min, double max)
      {
         var range = populationSettings.ParameterRange(parameterName);
         range.MinValue = valueFrom(min);
         range.MaxValue = valueFrom(max);
      }

      //matlab might not support nullable. Hence we adopt the convention that NaN in matlab is null in .NET
      private double? valueFrom(double valueToConvert)
      {
         if (double.IsNaN(valueToConvert))
            return null;
         return valueToConvert;
      }
   }
}