using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;

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
            SpeciesPopulation = individualSettingsDTO.Population,
            Gender = individualSettingsDTO.Gender,
            SubPopulation = subPopulationFrom(individualSettingsDTO.SubPopulation),
            Age = originDataParameterFrom(individualSettingsDTO.ParameterAge),
            GestationalAge = originDataParameterFrom(individualSettingsDTO.ParameterGestationalAge),
            Height = originDataParameterFrom(individualSettingsDTO.ParameterHeight),
            Weight = originDataParameterFrom(individualSettingsDTO.ParameterWeight),
            BMI = originDataParameterFrom(individualSettingsDTO.ParameterBMI)
         };

         originData.UpdateValueOriginFrom(individualSettingsDTO.ValueOrigin);
         individualSettingsDTO.CalculationMethods.Select(cm => cm.CalculationMethod).Each(originData.AddCalculationMethod);

         return originData;
      }

      private OriginDataParameter originDataParameterFrom(IParameterDTO parameterDTO)
      {
         return new OriginDataParameter(parameterDTO.KernelValue, displayUnit(parameterDTO));
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