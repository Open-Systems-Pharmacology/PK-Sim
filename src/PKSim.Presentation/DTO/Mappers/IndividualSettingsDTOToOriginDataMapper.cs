using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualSettingsDTOToOriginDataMapper : IMapper<IndividualSettingsDTO, OriginData>
   {
   }

   public class IndividualSettingsDTOToOriginDataMapper : IIndividualSettingsDTOToOriginDataMapper
   {
      private readonly IDiseaseStateUpdater _diseaseStateUpdater;
      private readonly IParameterDTOToOriginDataParameterMapper _originDataParameterMapper;

      public IndividualSettingsDTOToOriginDataMapper(
         IDiseaseStateUpdater diseaseStateUpdater, 
         IParameterDTOToOriginDataParameterMapper originDataParameterMapper)
      {
         _diseaseStateUpdater = diseaseStateUpdater;
         _originDataParameterMapper = originDataParameterMapper;
      }

      public OriginData MapFrom(IndividualSettingsDTO individualSettingsDTO)
      {
         var originData = new OriginData
         {
            Species = individualSettingsDTO.Species,
            Population = individualSettingsDTO.Population,
            Gender = individualSettingsDTO.Gender,
            SubPopulation = subPopulationFrom(individualSettingsDTO.SubPopulation),
            Age = originDataParameterFrom(individualSettingsDTO.ParameterAge),
            GestationalAge = originDataParameterFrom(individualSettingsDTO.ParameterGestationalAge),
            Height = originDataParameterFrom(individualSettingsDTO.ParameterHeight),
            Weight = originDataParameterFrom(individualSettingsDTO.ParameterWeight),
            BMI = originDataParameterFrom(individualSettingsDTO.ParameterBMI)
         };

         _diseaseStateUpdater.UpdateOriginDataFromDiseaseState(originData, individualSettingsDTO.DiseaseState);

         originData.UpdateValueOriginFrom(individualSettingsDTO.ValueOrigin);
         individualSettingsDTO.CalculationMethods.Select(cm => cm.CalculationMethod).Each(originData.AddCalculationMethod);

         return originData;
      }

      private OriginDataParameter originDataParameterFrom(IParameterDTO parameterDTO) => _originDataParameterMapper.MapFrom(parameterDTO, addName: false);

      private SubPopulation subPopulationFrom(IEnumerable<CategoryParameterValueVersionDTO> subPopulationDTO)
      {
         var subPopulation = new SubPopulation();
         subPopulationDTO.Select(x => x.ParameterValueVersion).Each(subPopulation.AddParameterValueVersion);
         return subPopulation;
      }
   }
}