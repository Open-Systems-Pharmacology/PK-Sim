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

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class IndividualMoleculeContextMenu : ContextMenu<IndividualMolecule, IMoleculesPresenter>
   {
      public IndividualMoleculeContextMenu(IndividualMolecule molecule, IMoleculesPresenter moleculesPresenter)
         : base(molecule, moleculesPresenter)
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
      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<IMoleculesPresenter>();
      }

      public override IContextMenu CreateFor(IndividualMolecule individualMolecule, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualMoleculeContextMenu(individualMolecule, presenter.DowncastTo<IMoleculesPresenter>());
      }
   }
}