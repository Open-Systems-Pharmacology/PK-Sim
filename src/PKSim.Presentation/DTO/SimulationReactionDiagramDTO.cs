using System.Collections;
using System.Collections.Generic;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation.DTO
{
   public class SimulationReactionDiagramDTO : IWithDiagramFor<SimulationReactionDiagramDTO>, IEnumerable<ReactionBuilder>
   {
      public virtual IDiagramModel DiagramModel { get; set; }

      public virtual IDiagramManager<SimulationReactionDiagramDTO> DiagramManager { get; set; }

      public virtual ReactionBuildingBlock ReactionBuildingBlock { get; set; }

      public virtual IEnumerator<ReactionBuilder> GetEnumerator()
      {
         return ReactionBuildingBlock.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}