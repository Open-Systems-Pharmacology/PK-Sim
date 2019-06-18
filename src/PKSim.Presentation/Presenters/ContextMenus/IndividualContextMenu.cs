using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class IndividualContextMenu : BuildingBlockContextMenu<Individual>
   {
      public IndividualContextMenu(Individual individual) : base(individual)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Individual individual)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<EditIndividualCommand>(individual));
         allMenuItems.Add(scaleIndividualMenuFor(individual));
         allMenuItems.Add(newPopulationMenuFor(individual));
         allMenuItems.AddRange(ExportAndDeleteContextMenusFor(individual));
         return allMenuItems;
      }

      private static IMenuBarButton newPopulationMenuFor(Individual individual)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewPopulation)
            .WithIcon(ApplicationIcons.Population)
            .WithCommandFor<CreatePopulationBasedOnIndividualCommand, Individual>(individual);
      }

      private static IMenuBarButton scaleIndividualMenuFor(Individual individual)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Scale)
            .WithIcon(ApplicationIcons.ScaleIndividual)
            .WithCommandFor<ScaleIndividualCommand, Individual>(individual)
            .AsGroupStarter();
      }
   }

   public class IndividualTreeNodeContextMenuFactory : NodeContextMenuFactory<Individual>
   {
      public override IContextMenu CreateFor(Individual individual, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualContextMenu(individual);
      }
   }
}