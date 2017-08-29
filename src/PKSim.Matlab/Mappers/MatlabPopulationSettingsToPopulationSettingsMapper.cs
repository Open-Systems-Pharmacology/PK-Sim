using System.Linq;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Matlab.Mappers
{
   public interface IMatlabPopulationSettingsToPopulationSettingsMapper : IMapper<PopulationSettings, RandomPopulationSettings>
   {
   }

   public class MatlabPopulationSettingsToPopulationSettingsMapper : IMatlabPopulationSettingsToPopulationSettingsMapper
   {
      private readonly IMatlabOriginDataToOriginDataMapper _originDataMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualToPopulationSettingsMapper _individualToPopulationSettingsMapper;
      private readonly IGenderRepository _genderRepository;

      public MatlabPopulationSettingsToPopulationSettingsMapper(IMatlabOriginDataToOriginDataMapper originDataMapper, IIndividualFactory individualFactory,
         IIndividualToPopulationSettingsMapper individualToPopulationSettingsMapper, IGenderRepository genderRepository)
      {
         _originDataMapper = originDataMapper;
         _individualFactory = individualFactory;
         _individualToPopulationSettingsMapper = individualToPopulationSettingsMapper;
         _genderRepository = genderRepository;
      }

      public MatlabPopulationSettingsToPopulationSettingsMapper()
      {
      }

      public RandomPopulationSettings MapFrom(PopulationSettings matlabPopulationSettings)
      {
         var originData = _originDataMapper.MapFrom(matlabPopulationSettings);

         var individual = _individualFactory.CreateStandardFor(originData);
         var populationSettings = _individualToPopulationSettingsMapper.MapFrom(individual);
         populationSettings.NumberOfIndividuals = matlabPopulationSettings.NumberOfIndividuals;

         setRange(populationSettings, CoreConstants.Parameter.AGE, matlabPopulationSettings.MinAge, matlabPopulationSettings.MaxAge);
         setRange(populationSettings, CoreConstants.Parameter.MEAN_HEIGHT, matlabPopulationSettings.MinHeight, matlabPopulationSettings.MaxHeight);
         setRange(populationSettings, CoreConstants.Parameter.MEAN_WEIGHT, matlabPopulationSettings.MinWeight, matlabPopulationSettings.MaxWeight);
         setRange(populationSettings, CoreConstants.Parameter.BMI, matlabPopulationSettings.MinBMI, matlabPopulationSettings.MaxBMI);

         if (populationSettings.ContainsParameterRangeFor(CoreConstants.Parameter.GESTATIONAL_AGE))
            setRange(populationSettings, CoreConstants.Parameter.GESTATIONAL_AGE, matlabPopulationSettings.MinGestationalAge, matlabPopulationSettings.MaxGestationalAge);

         //in case of multiple gender, adjust the ration according to the feamales proportion
         if (individual.AvailableGenders().Count() > 1)
         {
            populationSettings.GenderRatio(_genderRepository.Female).Ratio = matlabPopulationSettings.ProportionOfFemales;
            populationSettings.GenderRatio(_genderRepository.Male).Ratio = 100 - matlabPopulationSettings.ProportionOfFemales;
         }

         return populationSettings;
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