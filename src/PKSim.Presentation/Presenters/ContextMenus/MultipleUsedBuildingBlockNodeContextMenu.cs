using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleUsedBuildingBlockNodeContextMenu : MultipleBuildingBlockNodeContextMenu<IPKSimBuildingBlock>
   {
      public MultipleUsedBuildingBlockNodeContextMenu(IReadOnlyList<IPKSimBuildingBlock> buildingBlocks, IExecutionContext executionContext, IContainer container) : base(buildingBlocks, executionContext, container)
      {
      }

      public MultipleUsedBuildingBlockNodeContextMenu(IReadOnlyList<NamedBuildingBlock<IPKSimBuildingBlock>> buildingBlocks, IExecutionContext executionContext, IContainer container) : base(buildingBlocks, executionContext, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<NamedBuildingBlock<IPKSimBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
      {
         yield return CompareBuildingBlocks(buildingBlocks, executionContext);

         yield return AddToJournal(buildingBlocks);

         yield return DeleteSelectedBuildingBlockMenuItem(buildingBlocks);
      }
   }

   public class MultipleUsedBuildingBlockNodeContextMenuFactory : MultipleNodeContextMenuFactory<UsedBuildingBlock>
   {
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleUsedBuildingBlockNodeContextMenuFactory(IBuildingBlockInProjectManager buildingBlockInProjectManager, IExecutionContext executionContext, IContainer container)
      {
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<UsedBuildingBlock> usedBuildingBlock, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         var buildingBlocks = loadedBuildingBlocksBasedOn(usedBuildingBlock);
         return new MultipleUsedBuildingBlockNodeContextMenu(buildingBlocks, _executionContext, _container);
      }

      private IReadOnlyList<NamedBuildingBlock<IPKSimBuildingBlock>> loadedBuildingBlocksBasedOn(IEnumerable<UsedBuildingBlock> usedBuildingBlocks)
      {
         var loadedBuildingBlocks = new List<NamedBuildingBlock<IPKSimBuildingBlock>>();
         foreach (var usedBuildingBlock in usedBuildingBlocks)
         {
            var simulation = _buildingBlockInProjectManager.SimulationUsing(usedBuildingBlock);
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