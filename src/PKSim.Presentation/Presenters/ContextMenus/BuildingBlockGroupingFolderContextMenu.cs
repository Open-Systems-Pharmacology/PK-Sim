using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Utility.Extensions;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus;

internal class BuildingBlockGroupingFolderContextMenu : ExplorerClassificationNodeContextMenu
{
   public BuildingBlockGroupingFolderContextMenu(ClassificationNode objectRequestingContextMenu, IExplorerPresenter presenter, IContainer container)
      : base(objectRequestingContextMenu, presenter, container)
   {
   }
}

/// <summary>
///    Provides the right-click menu for the classification (subfolder) nodes created under any of the building
///    block folders. One factory handles all building block classification types.
/// </summary>
public class BuildingBlockGroupingFolderTreeNodeContextMenuFactory :
   IContextMenuSpecificationFactory<ITreeNode>,
   IContextMenuSpecificationFactory<IViewItem>
{
   private readonly IContainer _container;

   private static readonly HashSet<ClassificationType> _buildingBlockClassificationTypes = new HashSet<ClassificationType>
   {
      ClassificationType.Compound,
      ClassificationType.Formulation,
      ClassificationType.Individual,
      ClassificationType.Population,
      ClassificationType.Protocol,
      ClassificationType.Event,
      ClassificationType.ObserverSet,
      ClassificationType.ExpressionProfile
   };

   public BuildingBlockGroupingFolderTreeNodeContextMenuFactory(IContainer container)
   {
      _container = container;
   }

   public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter) => createFor(treeNode, presenter);

   public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter) => isSatisfiedBy(treeNode, presenter);

   public IContextMenu CreateFor(IViewItem viewItem, IPresenterWithContextMenu<IViewItem> presenter) => createFor(viewItem, presenter);

   public bool IsSatisfiedBy(IViewItem viewItem, IPresenterWithContextMenu<IViewItem> presenter) => isSatisfiedBy(viewItem, presenter);

   private IContextMenu createFor(object nodeRequestingContextMenu, IPresenter presenter) =>
      new BuildingBlockGroupingFolderContextMenu(nodeRequestingContextMenu.DowncastTo<ClassificationNode>(), presenter.DowncastTo<IExplorerPresenter>(), _container);

   private static bool isSatisfiedBy(object nodeRequestingContextMenu, IPresenter presenter)
   {
      if (!nodeRequestingContextMenu.IsAnImplementationOf<ClassificationNode>())
         return false;

      var node = nodeRequestingContextMenu.DowncastTo<ClassificationNode>();
      return _buildingBlockClassificationTypes.Contains(node.Tag.ClassificationType) && presenter.IsAnImplementationOf<IExplorerPresenter>();
   }
}
