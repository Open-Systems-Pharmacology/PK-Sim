using System.Collections.Generic;
using BTS.UI.MenuAndBars;
using BTS.UI.TreeList;
using BTS.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;
using PKSim.Resources;
using SBSuite.Core.Domain;
using SBSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SummaryChartContextMenu : ContextMenu<ISimulationComparison>
   {
      public SummaryChartContextMenu(ISimulationComparison summaryChart) : base(summaryChart)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ISimulationComparison summaryChart)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Edit)
            .WithCommandFor<EditSubjectUICommand<ISimulationComparison>, ISimulationComparison>(summaryChart)
            .WithIcon(ApplicationIcons.Edit)
            .EnabledInReadOnly();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithCommandFor<RenameObjectUICommand, IWithName>(summaryChart)
            .WithIcon(ApplicationIcons.Rename);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<RemoveSummaryChartUICommand, ISimulationComparison>(summaryChart)
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class SummaryChartTreeNodeContextMenuFactory : NodeContextMenuFactory<ISimulationComparison>
   {
      public override IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SummaryChartContextMenu(treeNode.TagAsObject.DowncastTo<ISimulationComparison>());
      }
   }
}