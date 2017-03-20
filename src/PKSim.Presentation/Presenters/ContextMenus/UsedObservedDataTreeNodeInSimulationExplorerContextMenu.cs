using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class UsedObservedDataTreeNodeInSimulationExplorerContextMenu : ContextMenu<UsedObservedData>
   {
      public UsedObservedDataTreeNodeInSimulationExplorerContextMenu(UsedObservedData usedObservedData)
         : base(usedObservedData)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(UsedObservedData usedObservedData)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Remove)
            .WithCommand(IoC.Resolve<DeleteUsedObservedDataUICommand>().For(usedObservedData))
            .WithIcon(ApplicationIcons.Remove)
            .AsGroupStarter();
      }
   }

   public class UsedObservedDataTreeNodeInSimulationExplorerContextMenuFactory : IContextMenuSpecificationFactory<ITreeNode>
   {
      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return treeNode.IsAnImplementationOf<UsedObservedDataNode>() &&
                presenter.IsAnImplementationOf<ISimulationExplorerPresenter>();
      }

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         var usedObservedDataNode = treeNode.DowncastTo<UsedObservedDataNode>();
         return new UsedObservedDataTreeNodeInSimulationExplorerContextMenu(usedObservedDataNode.Tag);
      }
   }
}