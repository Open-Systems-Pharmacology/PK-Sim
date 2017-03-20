using System.Collections;
using System.Collections.Generic;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation.DTO
{
   public class SimulationReactionDiagramDTO : IWithDiagramFor<SimulationReactionDiagramDTO>, IEnumerable<IReactionBuilder>
   {
      public virtual IDiagramModel DiagramModel { get; set; }

      public virtual IDiagramManager<SimulationReactionDiagramDTO> DiagramManager { get; set; }

      public virtual IReactionBuildingBlock ReactionBuildingBlock { get; set; }

      public virtual IEnumerator<IReactionBuilder> GetEnumerator()
      {
         return ReactionBuildingBlock.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}