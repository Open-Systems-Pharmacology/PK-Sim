using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class IndividualMoleculesContextMenu<TMolecule> : ContextMenu where TMolecule : IndividualMolecule

   {
      private readonly string _addProteinCaption;
      private readonly ApplicationIcon _addProteinIcon;

      protected IndividualMoleculesContextMenu(IMoleculesPresenter presenter, string addProteinCaption, ApplicationIcon addProteinIcon, IContainer container) : base(container)
      {
         _addProteinCaption = addProteinCaption;
         _addProteinIcon = addProteinIcon;
         allMenuItemsFor(presenter).Each(_view.AddMenuItem);
      }

      private IEnumerable<IMenuBarItem> allMenuItemsFor(IMoleculesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(_addProteinCaption)
               .WithActionCommand(presenter.AddMolecule<TMolecule>)
               .WithIcon(_addProteinIcon);

      }
   }

   public class EnzymesContextMenu : IndividualMoleculesContextMenu<IndividualEnzyme>
   {
      public EnzymesContextMenu(IMoleculesPresenter presenter, IContainer container)
         : base(presenter, PKSimConstants.UI.AddMetabolizingEnzyme, ApplicationIcons.Enzyme, container)
      {
      }
   }

   public class OtherProteinsContextMenu : IndividualMoleculesContextMenu<IndividualOtherProtein>
   {
      public OtherProteinsContextMenu(IMoleculesPresenter presenter, IContainer container)
         : base(presenter, PKSimConstants.UI.AddSpecificBindingPartner, ApplicationIcons.Protein, container)
      {
      }
   }

   public class TransportersContextMenu : IndividualMoleculesContextMenu<IndividualTransporter>
   {
      public TransportersContextMenu(IMoleculesPresenter presenter, IContainer container)
         : base(presenter, PKSimConstants.UI.AddTransportProtein, ApplicationIcons.Transporter, container)
      {
      }
   }

   public abstract class IndividualMoleculesContextMenuContextMenuFactory : RootNodeContextMenuFactory
   {
      protected IndividualMoleculesContextMenuContextMenuFactory(RootNodeType rootNodeType, IMenuBarItemRepository repository)
         : base(rootNodeType, repository)
      {
      }

      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<IMoleculesPresenter>();
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return CreateFor(treeNode.DowncastTo<RootNode>(), presenter.DowncastTo<IMoleculesPresenter>());
      }

      public abstract IContextMenu CreateFor(RootNode proteinsNode, IMoleculesPresenter presenter);
   }

   public class EnzymesTreeNodeContextMenuFactory : IndividualMoleculesContextMenuContextMenuFactory
   {
      private readonly IContainer _container;

      public EnzymesTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(PKSimRootNodeTypes.IndividualMetabolizingEnzymes, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(RootNode proteinsNode, IMoleculesPresenter moleculesPresenter)
      {
         return new EnzymesContextMenu(moleculesPresenter, _container);
      }
   }

   public class OtherProteinsContextMenuFactory : IndividualMoleculesContextMenuContextMenuFactory
   {
      private readonly IContainer _container;

      public OtherProteinsContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(PKSimRootNodeTypes.IndividualProteinBindingPartners, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(RootNode proteinsNode, IMoleculesPresenter presenter)
      {
         return new OtherProteinsContextMenu(presenter, _container);
      }
   }

   public class TransportersContextMenuFactory : IndividualMoleculesContextMenuContextMenuFactory
   {
      private readonly IContainer _container;

      public TransportersContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(PKSimRootNodeTypes.IndividualTransportProteins, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(RootNode proteinsNode, IMoleculesPresenter presenter)
      {
         return new TransportersContextMenu(presenter, _container);
      }
   }
}