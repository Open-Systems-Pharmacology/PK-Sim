using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Assets;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationFavoritesNodeContextMenu : ContextMenu<IGroup, IParameterGroupsPresenter>
   {
      public SimulationFavoritesNodeContextMenu(IGroup group, IParameterGroupsPresenter presenter, IContainer container) : base(group, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IGroup group, IParameterGroupsPresenter presenter)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.StartParameterIdentification)
            .WithCommandFor<CreateParameterIdentificationBasedOnParametersUICommand, IEnumerable<IParameter>>(presenter.AllParametersInSelectedGroup, _container)
            .WithIcon(ApplicationIcons.ParameterIdentification);
      }
   }

   public class SimulationFavoritesNodeContextMenuFactory : NodeContextMenuFactory<IGroup>
   {
      private readonly IContainer _container;

      public SimulationFavoritesNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) &&
                nodeIsFavoriteNode(treeNode) &&
                presenterIsSimulationGroupNodePresenterWithSimulationParameters(presenter);
      }

      private bool presenterIsSimulationGroupNodePresenterWithSimulationParameters(IPresenterWithContextMenu<ITreeNode> presenter)
      {
         var groupNodePresenter = presenter as IParameterGroupsPresenter;
         if (groupNodePresenter == null)
            return false;

         var parameters = groupNodePresenter.AllParametersInSelectedGroup.ToList();
         if (!parameters.Any())
            return false;

         return parameters.All(x => !string.IsNullOrEmpty(x.Origin.SimulationId));
      }

      private bool nodeIsFavoriteNode(ITreeNode treeNode)
      {
         return treeNode.TagAsObject.DowncastTo<IGroup>().IsNamed(CoreConstants.Groups.FAVORITES);
      }

      public override IContextMenu CreateFor(IGroup group, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SimulationFavoritesNodeContextMenu(group, presenter.DowncastTo<IParameterGroupsPresenter>(), _container);
      }
   }
}