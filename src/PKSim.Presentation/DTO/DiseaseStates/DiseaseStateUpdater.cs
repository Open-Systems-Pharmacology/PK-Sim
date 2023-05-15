using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.DiseaseStates
{
   public interface IDiseaseStateUpdater
   {
      /// <summary>
      ///    Updates the value in the DTO based on the origin data value (typically used in edit scenario)
      /// </summary>
      void UpdateDiseaseStateDTO(DiseaseStateDTO diseaseStateDTO, OriginData originData);

      /// <summary>
      ///    Updates the origin data with value from the DTO. This is typically used in save scenario
      /// </summary>
      void UpdateOriginDataFromDiseaseState(OriginData originData, DiseaseStateDTO diseaseStateDTO);

      void UpdateDiseaseStateParameters(DiseaseStateDTO diseaseStateDTO);
   }

   public class DiseaseStateUpdater : IDiseaseStateUpdater
   {
      private readonly ICloner _cloner;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly IDiseaseStateRepository _diseaseStateRepository;
      private readonly IOriginDataParameterToParameterDTOMapper _originDataParameterMapper;
      private readonly IParameterDTOToOriginDataParameterMapper _parameterDTOToOriginDataMapper;

      public DiseaseStateUpdater(
         ICloner cloner,
         IParameterToParameterDTOMapper parameterMapper,
         IDiseaseStateRepository diseaseStateRepository,
         IOriginDataParameterToParameterDTOMapper originDataParameterMapper,
         IParameterDTOToOriginDataParameterMapper parameterDTOToOriginDataMapper)
      {
         _cloner = cloner;
         _parameterMapper = parameterMapper;
         _diseaseStateRepository = diseaseStateRepository;
         _originDataParameterMapper = originDataParameterMapper;
         _parameterDTOToOriginDataMapper = parameterDTOToOriginDataMapper;
      }

      public void UpdateDiseaseStateDTO(DiseaseStateDTO diseaseStateDTO, OriginData originData)
      {
         var diseaseState = originData.DiseaseState ?? _diseaseStateRepository.HealthyState;
         diseaseStateDTO.Value = diseaseState;
         diseaseStateDTO.Parameter = originData.DiseaseStateParameters
            .Select(_originDataParameterMapper.MapFrom)
            .FirstOrDefault() ?? new NullParameterDTO();
      }

      public void UpdateOriginDataFromDiseaseState(OriginData originData, DiseaseStateDTO diseaseStateDTO)
      {
         var diseaseState = diseaseStateDTO.Value;
         var isHealthy = diseaseState.IsHealthy;
         if (isHealthy)
         {
            originData.DiseaseState = null;
            return;
         }

         originData.DiseaseState = diseaseState;
         var diseaseStateParameter = diseaseStateDTO.Parameter;
         //This is a disease state without parameters
         if (diseaseStateParameter.IsNull())
            return;

         //disease parameters are saved in a collection and we need to save the name to differentiate them
         originData.AddDiseaseStateParameter(_parameterDTOToOriginDataMapper.MapFrom(diseaseStateParameter, addName: true));
      }

      public void UpdateDiseaseStateParameters(DiseaseStateDTO diseaseStateDTO)
      {
         //can be null if switching from a species/pop that supports disease state to one that does not support it
         var diseaseState = diseaseStateDTO.Value;
         //We clone parameters to ensure that we are not updating the default from DB;
         diseaseStateDTO.Parameter = diseaseState?.Parameters
            .Select(_cloner.Clone)
            .Select(_parameterMapper.MapAsReadWriteFrom)
            .FirstOrDefault() ?? new NullParameterDTO();
      }
   }
}