using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IEnzymaticProcessToEnzymaticProcessDTOMapper : IMapper<EnzymaticProcess, EnzymaticProcessDTO>
   {
   }

   public class EnzymaticProcessToEnzymaticProcessDTOMapper : CompoundProcessToCompoundProcessDTOMapper, IEnzymaticProcessToEnzymaticProcessDTOMapper
   {
      public EnzymaticProcessToEnzymaticProcessDTOMapper(IRepresentationInfoRepository representationInfoRepository)
         : base(representationInfoRepository)
      {
      }

      public EnzymaticProcessDTO MapFrom(EnzymaticProcess compoundProcess)
      {
         var dto = new EnzymaticProcessDTO(compoundProcess);
         SetProperties(compoundProcess, dto);
         dto.Metabolite = compoundProcess.MetaboliteName;
         return dto;
      }
   }
}