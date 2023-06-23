using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;

using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SystemicProcessFolderContextMenu : ContextMenu<SystemicProcessNodeType, ICompoundProcessesPresenter>
   {
      public SystemicProcessFolderContextMenu(SystemicProcessNodeType systemicProcessNodeType, ICompoundProcessesPresenter presenter, IContainer container)
         : base(systemicProcessNodeType, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(SystemicProcessNodeType systemicProcessNodeType, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddPartialProcess(systemicProcessNodeType.Name))
               .WithActionCommand(() => presenter.AddSystemicProcess(systemicProcessNodeType.SystemicTypes))
               .WithIcon(systemicProcessNodeType.Icon);
      }
   }

   public abstract class SystemicProcessFolderTreeNodeContextMenuFactory : IContextMenuSpecificationFactory<ITreeNode>
   {
      private readonly SystemicProcessNodeType _systemicProcessNodeType;
      private readonly IContainer _container;

      protected SystemicProcessFolderTreeNodeContextMenuFactory(SystemicProcessNodeType systemicProcessNodeType, IContainer container)
      {
         _systemicProcessNodeType = systemicProcessNodeType;
         _container = container;
      }

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         var systemicProcessNode = treeNode as ITreeNode<SystemicProcessNodeType>;
         if (!presenter.IsAnImplementationOf<ICompoundProcessesPresenter>()) return false;
         return systemicProcessNode != null && _systemicProcessNodeType.Equals(systemicProcessNode.Tag);
      }

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SystemicProcessFolderContextMenu(_systemicProcessNodeType, presenter.DowncastTo<ICompoundProcessesPresenter>(), _container);
      }
   }

   public class HepaticSystemicProcessFolderTreeNodeContextMenuFactory : SystemicProcessFolderTreeNodeContextMenuFactory
   {
      public HepaticSystemicProcessFolderTreeNodeContextMenuFactory(IContainer container) : base(SystemicProcessNodeType.HepaticClearance, container)
      {
      }
   }

   public class RenalSystemicProcessFolderTreeNodeContextMenuFactory : SystemicProcessFolderTreeNodeContextMenuFactory
   {
      public RenalSystemicProcessFolderTreeNodeContextMenuFactory(IContainer container) : base(SystemicProcessNodeType.RenalClearance, container)
      {
      }
   }

   public class BiliarySystemicProcessFolderTreeNodeContextMenuFactory : SystemicProcessFolderTreeNodeContextMenuFactory
   {
      public BiliarySystemicProcessFolderTreeNodeContextMenuFactory(IContainer container) : base(SystemicProcessNodeType.BiliaryClearance, container)
      {
      }
   }

}