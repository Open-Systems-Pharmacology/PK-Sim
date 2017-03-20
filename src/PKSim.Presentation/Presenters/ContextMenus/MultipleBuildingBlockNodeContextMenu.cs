using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class MultipleBuildingBlockNodeContextMenu<TBuildingBlock> : ContextMenu<IReadOnlyList<NamedBuildingBlock<TBuildingBlock>>, IExecutionContext> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected MultipleBuildingBlockNodeContextMenu(IReadOnlyList<TBuildingBlock> buildingBlocks, IExecutionContext executionContext)
         : this(buildingBlocks.Select(bb => new NamedBuildingBlock<TBuildingBlock>(bb, bb.Name)).ToList(), executionContext)
      {
      }

      protected MultipleBuildingBlockNodeContextMenu(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
      {
         yield return CompareBuidlingBlocks(buildingBlocks, executionContext);

         yield return AddToJournal(buildingBlocks);

         yield return DeleteSelectedBuildingBlockMenuItem(buildingBlocks);
      }

      public static IMenuBarItem CompareBuidlingBlocks(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
      {
         var buildingBlockList = buildingBlocks.Select(x => x.BuildingBlock).ToList();
         var objectBaseList = buildingBlockList.Cast<IObjectBase>().ToList();

         var buildingBlockNames = buildingBlocks.Select(x => x.Name).ToList();

         if (canStartComparisonFor(buildingBlockList, executionContext))
            return ComparisonCommonContextMenuItems.CompareObjectsMenu(objectBaseList, buildingBlockNames, executionContext);
         else
            return comparisonNotPossibleMenu(buildingBlockList, executionContext);
      }

      protected static IMenuBarItem AddToJournal(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks)
      {
         var objectBaseList = buildingBlocks.Select(x=>x.BuildingBlock).Cast<IObjectBase>().ToList();

         return ObjectBaseCommonContextMenuItems.AddToJournal(objectBaseList);
      }

      protected static IMenuBarButton DeleteSelectedBuildingBlockMenuItem(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks)
      {
         var buildingBlockList = buildingBlocks.Select(x => x.BuildingBlock).ToList();

         return CreateMenuButton.WithCaption(MenuNames.Delete)
            .WithCommandFor<DeleteBuildingBlocksUICommand, IReadOnlyList<IPKSimBuildingBlock>>(buildingBlockList)
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }

      private static bool canStartComparisonFor(IReadOnlyList<TBuildingBlock> buildingBlocks, IExecutionContext context)
      {
         if (buildingBlocks.Count != 2)
            return false;

         var simulationBuildingBlockUpdater = context.Resolve<ISimulationBuildingBlockUpdater>();
         return simulationBuildingBlockUpdater.BuildingBlockSupportsQuickUpdate(buildingBlocks[0]);
      }

      private static IMenuBarButton comparisonNotPossibleMenu(IReadOnlyList<TBuildingBlock> buildingBlocks,IExecutionContext executionContext)
      {
         var buiildingBlockType = executionContext.TypeFor(buildingBlocks[0]);
         return CreateMenuButton.WithCaption(MenuNames.CompareObjects(executionContext.TypeFor(buildingBlocks[0])))
            .WithIcon(ApplicationIcons.Comparison)
            .WithActionCommand(() => { throw new PKSimException(PKSimConstants.Error.ComparisonBetweenBuildingBLocksNotSupportedForBuildingBlockOfType(buiildingBlockType)); });
      }
   }

   public class MultipleBuildingBlockNodeContextMenu : MultipleBuildingBlockNodeContextMenu<IPKSimBuildingBlock>
   {
      public MultipleBuildingBlockNodeContextMenu(IReadOnlyList<IPKSimBuildingBlock> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }

      public MultipleBuildingBlockNodeContextMenu(IReadOnlyList<NamedBuildingBlock<IPKSimBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }
   }

   public class MultipleBuildingBlockNodeContextMenuFactory : MultipleNodeContextMenuFactory<IPKSimBuildingBlock>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleBuildingBlockNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<IPKSimBuildingBlock> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleBuildingBlockNodeContextMenu(buildingBlocks, _executionContext);
      }
   }
}