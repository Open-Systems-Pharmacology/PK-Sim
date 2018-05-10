using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualSettingsDTOToOriginDataMapper : IMapper<IndividualSettingsDTO, OriginData>
   {
   }

   public class IndividualSettingsDTOToOriginDataMapper : IIndividualSettingsDTOToOriginDataMapper
   {
      public OriginData MapFrom(IndividualSettingsDTO individualSettingsDTO)
      {
         var originData = new OriginData
                             {
                                Species = individualSettingsDTO.Species,
                                SpeciesPopulation = individualSettingsDTO.SpeciesPopulation,
                                Gender = individualSettingsDTO.Gender,
                                SubPopulation = subPopulationFrom(individualSettingsDTO.SubPopulation),
                                Age = individualSettingsDTO.ParameterAge.KernelValue,
                                AgeUnit = displayUnit(individualSettingsDTO.ParameterAge),
                                GestationalAge = individualSettingsDTO.ParameterGestationalAge.KernelValue,
                                GestationalAgeUnit = displayUnit(individualSettingsDTO.ParameterGestationalAge),
                                Height = individualSettingsDTO.ParameterHeight.KernelValue,
                                HeightUnit = displayUnit(individualSettingsDTO.ParameterHeight),
                                Weight = individualSettingsDTO.ParameterWeight.KernelValue,
                                WeightUnit = displayUnit(individualSettingsDTO.ParameterWeight),
                                BMI = individualSettingsDTO.ParameterBMI.KernelValue,
                                BMIUnit = displayUnit(individualSettingsDTO.ParameterBMI),
                             };

         originData.UpdateValueOriginFrom(individualSettingsDTO.ValueOrigin);
         individualSettingsDTO.CalculationMethods.Select(cm => cm.CalculationMethod).Each(originData.AddCalculationMethod);

         return originData;
      }

      private static string displayUnit(IParameterDTO parameterDTO)
      {
         if (parameterDTO.DisplayUnit == null)
            return string.Empty;
         return parameterDTO.DisplayUnit.ToString();
      }

      private SubPopulation subPopulationFrom(IEnumerable<CategoryParameterValueVersionDTO> subPopulationDTO)
      {
         var subPopulation = new SubPopulation();
         subPopulationDTO.Select(x => x.ParameterValueVersion).Each(subPopulation.AddParameterValueVersion);
         return subPopulation;
      }
   }
}