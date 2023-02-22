using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class IndividualMoleculeContextMenu : ContextMenu<IndividualMolecule, IMoleculesPresenter>
   {
      public IndividualMoleculeContextMenu(IndividualMolecule molecule, IMoleculesPresenter moleculesPresenter, IContainer container)
         : base(molecule, moleculesPresenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IndividualMolecule molecule, IMoleculesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
               .WithActionCommand(() => presenter.RemoveMolecule(molecule))
               .WithIcon(ApplicationIcons.Delete)
               .AsGroupStarter();
      }
   }

   public class IndividualMoleculeTreeNodeContextMenuFactory : NodeContextMenuFactory<IndividualMolecule>
   {
      private readonly IContainer _container;

      public IndividualMoleculeTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<IMoleculesPresenter>();
      }

      public override IContextMenu CreateFor(IndividualMolecule individualMolecule, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualMoleculeContextMenu(individualMolecule, presenter.DowncastTo<IMoleculesPresenter>(), _container);
      }
   }
}