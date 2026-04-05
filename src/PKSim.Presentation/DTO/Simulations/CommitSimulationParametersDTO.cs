using System.Collections.Generic;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CommitSimulationParametersDTO
   {
      public List<CompoundCommitDTO> Compounds { get; init; } = new();
   }
}
