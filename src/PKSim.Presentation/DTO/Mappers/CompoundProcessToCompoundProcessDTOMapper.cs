using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ICompoundProcessToCompoundProcessDTOMapper : IMapper<CompoundProcess, CompoundProcessDTO>
   {
   }

   public class CompoundProcessToCompoundProcessDTOMapper : ICompoundProcessToCompoundProcessDTOMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository; 

      public CompoundProcessToCompoundProcessDTOMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public CompoundProcessDTO MapFrom(CompoundProcess compoundProcess)
      {
         var compoundProcessDTO = new CompoundProcessDTO(compoundProcess);

         SetProperties(compoundProcess, compoundProcessDTO);

         return compoundProcessDTO;
      }

      protected void SetProperties<TCompoundProcess>(TCompoundProcess compoundProcess, ProcessDTO<TCompoundProcess> compoundProcessDTO) where TCompoundProcess : CompoundProcess
      {
         compoundProcessDTO.Description = compoundProcess.Description;
         compoundProcessDTO.Name = compoundProcess.Name;
         compoundProcessDTO.ProcessTypeDisplayName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.PROCESS, compoundProcess.InternalName);
         compoundProcessDTO.Species = compoundProcess.Species;
      }
   }
}