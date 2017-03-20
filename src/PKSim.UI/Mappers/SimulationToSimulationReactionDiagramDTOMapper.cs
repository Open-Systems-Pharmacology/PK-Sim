using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.UI.Diagram.Managers;

namespace PKSim.UI.Mappers
{
   public class SimulationToSimulationReactionDiagramDTOMapper : ISimulationToSimulationReactionDiagramDTOMapper
   {
      private readonly IReactionBuildingBlockCreator _reactionBuildingBlockCreator;

      public SimulationToSimulationReactionDiagramDTOMapper(IReactionBuildingBlockCreator reactionBuildingBlockCreator)
      {
         _reactionBuildingBlockCreator = reactionBuildingBlockCreator;
      }

      public SimulationReactionDiagramDTO MapFrom(Simulation simulation)
      {
         var dto = new SimulationReactionDiagramDTO
         {
            DiagramModel = simulation.ReactionDiagramModel,
            ReactionBuildingBlock = simulation.Reactions ?? _reactionBuildingBlockCreator.CreateFor(simulation),
            DiagramManager = new ReactionDiagramManager<SimulationReactionDiagramDTO>()
         };

         reIdNodesFromNewBuildingBlock(dto.DiagramModel, dto.ReactionBuildingBlock);

         return dto;
      }

      private void reIdNodesFromNewBuildingBlock(IDiagramModel diagramModel, IEnumerable<IReactionBuilder> reactionBuildingBlock)
      {
         if (diagramModel == null || reactionBuildingBlock == null)
            return;

         var reIdentifiedNodes = new Dictionary<string, string>();

         reactionBuildingBlock.Each(builder =>
         {
            var oldNode = diagramModel.FindByName(builder.Name);
            if (oldNode == null) return;
            if (!string.Equals(oldNode.Id, builder.Id))
               reIdentifiedNodes.Add(oldNode.Id, builder.Id);
         });


         diagramModel.ReplaceNodeIds(reIdentifiedNodes);
      }
   }
}