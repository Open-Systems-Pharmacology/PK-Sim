using System.Collections.Generic;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.UI.Diagram.Managers;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.UI.Mappers
{
   public class SimulationToSimulationReactionDiagramDTOMapper : ISimulationToSimulationReactionDiagramDTOMapper
   {
      private readonly IReactionBuildingBlockCreator _reactionBuildingBlockCreator;
      private readonly IDiagramModelFactory _diagramModelFactory;

      public SimulationToSimulationReactionDiagramDTOMapper(
         IReactionBuildingBlockCreator reactionBuildingBlockCreator, IDiagramModelFactory diagramModelFactory)
      {
         _reactionBuildingBlockCreator = reactionBuildingBlockCreator;
         _diagramModelFactory = diagramModelFactory;
      }

      public SimulationReactionDiagramDTO MapFrom(Simulation simulation, bool recreateDiagram)
      {
         //Note: We do not use the building block in the simulation here as it MIGHT be out of date due to 
         //simulation being configured
         var reactionBuildingBlock = _reactionBuildingBlockCreator.CreateFor(simulation);
         var dto = new SimulationReactionDiagramDTO
         {
            DiagramModel = recreateDiagram ? _diagramModelFactory.Create() : simulation.ReactionDiagramModel,
            ReactionBuildingBlock = reactionBuildingBlock,
            DiagramManager = new ReactionDiagramManager<SimulationReactionDiagramDTO>()
         };

         reIdNodesFromNewBuildingBlock(dto.DiagramModel, dto.ReactionBuildingBlock);

         return dto;
      }

      private void reIdNodesFromNewBuildingBlock(IDiagramModel diagramModel, IEnumerable<ReactionBuilder> reactionBuildingBlock)
      {
         if (diagramModel == null || reactionBuildingBlock == null)
            return;

         var reIdentifiedNodes = new Dictionary<string, string>();

         reactionBuildingBlock.Each(reaction =>
         {
            var oldNode = diagramModel.FindByName(reaction.Name);
            if (oldNode == null) return;
            if (!string.Equals(oldNode.Id, reaction.Id))
               reIdentifiedNodes.Add(oldNode.Id, reaction.Id);
         });


         diagramModel.ReplaceNodeIds(reIdentifiedNodes);
      }
   }
}