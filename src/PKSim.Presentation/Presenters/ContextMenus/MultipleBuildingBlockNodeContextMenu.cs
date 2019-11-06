using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

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
         yield return SaveAsUserTemplateMenuFor(buildingBlocks);

         yield return SaveAsSystemTemplateMenuFor(buildingBlocks);

         yield return CompareBuildingBlocks(buildingBlocks, executionContext);

         yield return AddToJournal(buildingBlocks);

         yield return DeleteSelectedBuildingBlockMenuItem(buildingBlocks);
      }

      protected static IMenuBarItem CompareBuildingBlocks(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks, IExecutionContext executionContext)
      {
         var buildingBlockList = buildingBlocks.Select(x => x.BuildingBlock).ToList();
         var objectBaseList = buildingBlockList.Cast<IObjectBase>().ToList();

         var buildingBlockNames = buildingBlocks.Select(x => x.Name).ToList();

         if (canStartComparisonFor(buildingBlockList))
            return ComparisonCommonContextMenuItems.CompareObjectsMenu(objectBaseList, buildingBlockNames, executionContext);

         return comparisonNotPossibleMenu(buildingBlockList, executionContext);
      }

      protected IMenuBarItem SaveAsUserTemplateMenuFor(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks)
      {
         var buildingBlockList = buildingBlocks.Select(x => x.BuildingBlock).ToList();
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsTemplate)
            .WithCommandFor<SaveBuildingBlockAsTemplateCommand<TBuildingBlock>, IReadOnlyList<TBuildingBlock>>(buildingBlockList)
            .WithIcon(ApplicationIcons.SaveAsTemplate);
      }

      protected IMenuBarItem SaveAsSystemTemplateMenuFor(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks)
      {
         var buildingBlockList = buildingBlocks.Select(x => x.BuildingBlock).ToList();
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsSytemTemplate)
            .WithCommandFor<SaveBuildingBlockAsSystemTemplateCommand<TBuildingBlock>, IReadOnlyList<TBuildingBlock>>(buildingBlockList)
            .WithIcon(ApplicationIcons.SaveAsTemplate)
            .ForDeveloper();
      }

      protected static IMenuBarItem AddToJournal(IReadOnlyList<NamedBuildingBlock<TBuildingBlock>> buildingBlocks)
      {
         var objectBaseList = buildingBlocks.Select(x => x.BuildingBlock).Cast<IObjectBase>().ToList();

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

      private static bool canStartComparisonFor(IReadOnlyList<TBuildingBlock> buildingBlocks)
      {
         if (buildingBlocks.Count != 2)
            return false;

         return buildingBlocks[0].BuildingBlockType != PKSimBuildingBlockType.Population;
      }

      private static IMenuBarButton comparisonNotPossibleMenu(IReadOnlyList<TBuildingBlock> buildingBlocks, IExecutionContext executionContext)
      {
         var buildingBlockType = executionContext.TypeFor(buildingBlocks[0]);
         return CreateMenuButton.WithCaption(MenuNames.CompareObjects(executionContext.TypeFor(buildingBlocks[0])))
            .WithIcon(ApplicationIcons.Comparison)
            .WithActionCommand(() => throw new PKSimException(PKSimConstants.Error.ComparisonBetweenBuildingBLocksNotSupportedForBuildingBlockOfType(buildingBlockType)));
      }
   }
}