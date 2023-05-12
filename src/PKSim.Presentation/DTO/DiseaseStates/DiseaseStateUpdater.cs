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
      void UpdateDiseaseState(DiseaseStateDTO diseaseStateDTO, OriginData originData);
      void UpdateDiseaseStateParameters(DiseaseStateDTO diseaseStateDTO);
   }

   public class DiseaseStateUpdater : IDiseaseStateUpdater
   {
      private readonly ICloner _cloner;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly IDiseaseStateRepository _diseaseStateRepository;
      private readonly IOriginDataParameterToParameterDTOMapper _originDataParameterMapper;

      public DiseaseStateUpdater(
         ICloner cloner,
         IParameterToParameterDTOMapper parameterMapper,
         IDiseaseStateRepository diseaseStateRepository,
         IOriginDataParameterToParameterDTOMapper originDataParameterMapper)
      {
         _cloner = cloner;
         _parameterMapper = parameterMapper;
         _diseaseStateRepository = diseaseStateRepository;
         _originDataParameterMapper = originDataParameterMapper;
      }

      public void UpdateDiseaseState(DiseaseStateDTO diseaseStateDTO, OriginData originData)
      {
         var diseaseState = originData.DiseaseState ?? _diseaseStateRepository.HealthyState;
         diseaseStateDTO.Value = diseaseState;
         diseaseStateDTO.Parameter = originData.DiseaseStateParameters
            .Select(_originDataParameterMapper.MapFrom)
            .FirstOrDefault() ?? new NullParameterDTO();
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