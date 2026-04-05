using System.Collections.Generic;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CommitSimulationParametersDTO : DxValidatableDTO
   {
      public List<CompoundCommitDTO> Compounds { get; init; } = new();
   }
}
