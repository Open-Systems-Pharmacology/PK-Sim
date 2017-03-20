using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleUsedBuildingBlockNodeContextMenuFactory : MultipleNodeContextMenuFactory<UsedBuildingBlock>
   {
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly IExecutionContext _executionContext;

      public MultipleUsedBuildingBlockNodeContextMenuFactory(IBuildingBlockInSimulationManager buildingBlockInSimulationManager, IExecutionContext executionContext)
      {
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<UsedBuildingBlock> usedBuildingBlock, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         var buildingBlocks = loadedBuildingBlocksBasedOn(usedBuildingBlock);
         return new MultipleBuildingBlockNodeContextMenu(buildingBlocks, _executionContext);
      }

      private IReadOnlyList<NamedBuildingBlock<IPKSimBuildingBlock>> loadedBuildingBlocksBasedOn(IEnumerable<UsedBuildingBlock> usedBuildingBlocks)
      {
         var loadedBuildingBlocks = new List<NamedBuildingBlock<IPKSimBuildingBlock>>();
         foreach (var usedBuildingBlock in usedBuildingBlocks)
         {
            var simulation = _buildingBlockInSimulationManager.SimulationUsing(usedBuildingBlock);
            _executionContext.Load(simulation);
            var buildingBlock = simulation.UsedBuildingBlockById(usedBuildingBlock.Id).BuildingBlock;
            var name = CoreConstants.ContainerName.BuildingBlockInSimulationNameFor(buildingBlock.Name, simulation.Name);
            loadedBuildingBlocks.Add(new NamedBuildingBlock<IPKSimBuildingBlock>(buildingBlock, name));
         }

         return loadedBuildingBlocks;
      }

      public override bool IsSatisfiedBy(IReadOnlyList<ITreeNode> treeNodes, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         var allUsedBuildingBlocks = AllTagsFor(treeNodes);
         return base.IsSatisfiedBy(treeNodes, presenter) && allUsedBuildingBlocks.Select(x => x.BuildingBlockType).Distinct().Count() == 1;
      }
   }
}