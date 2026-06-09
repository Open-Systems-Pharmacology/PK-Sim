using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PartialProcessMoleculeContextMenu : ContextMenu<PartialProcessMoleculeNode, ICompoundProcessesPresenter>
   {
      public PartialProcessMoleculeContextMenu(PartialProcessMoleculeNode partialProcessMoleculeNode, ICompoundProcessesPresenter presenter, IContainer container)
         : base(partialProcessMoleculeNode, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PartialProcessMoleculeNode moleculeNode , ICompoundProcessesPresenter presenter)
      {
         yield return 
            CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithActionCommand(() => presenter.RenameMoleculeForPartialProcesses(moleculeNode.MoleculeName, moleculeNode.PartialProcessType))
            .WithIcon(ApplicationIcons.Rename);

         yield return
            CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddNewProcess)
            .WithActionCommand(() => presenter.AddPartialProcessesForMolecule(moleculeNode.MoleculeName, moleculeNode.PartialProcessType))
            .WithIcon(ApplicationIcons.Create);
      }
   }

   public class PartialProcessMoleculeTreeNodeContextMenuFactory : IContextMenuSpecificationFactory<ITreeNode>
   {
      private readonly IContainer _container;

      public PartialProcessMoleculeTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new PartialProcessMoleculeContextMenu(treeNode.DowncastTo<PartialProcessMoleculeNode>(), presenter.DowncastTo<ICompoundProcessesPresenter>(), _container);
      }

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return treeNode.IsAnImplementationOf<PartialProcessMoleculeNode>() && presenter.IsAnImplementationOf<ICompoundProcessesPresenter>();
      }
   }
}