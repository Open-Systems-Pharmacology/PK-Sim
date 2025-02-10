using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationComparisonContextMenu : ContextMenu<ISimulationComparison>
   {
      public SimulationComparisonContextMenu(ISimulationComparison summaryChart, IContainer container) : base(summaryChart, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ISimulationComparison simulationComparison)
      {
         yield return GenericMenu.EditMenuFor<EditSubjectUICommand<ISimulationComparison>, ISimulationComparison>(simulationComparison, _container);

         yield return GenericMenu.RenameMenuFor(simulationComparison, _container);

         yield return GenericMenu.EditDescriptionMenuFor(simulationComparison, _container)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Clone)
            .WithCommandFor<CloneSimulationComparisonCommand, ISimulationComparison>(simulationComparison, _container)
            .WithIcon(ApplicationIcons.Clone)
            .AsGroupStarter();


         var populationSimulationComparison = simulationComparison as PopulationSimulationComparison;
         if (populationSimulationComparison != null)
         {
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Configure)
               .WithCommandFor<ConfigurePopulationSimulationComparison, PopulationSimulationComparison>(populationSimulationComparison, _container)
               .AsGroupStarter();
         }

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<DeleteSimulationComparisonsUICommand, IReadOnlyList<ISimulationComparison>>(new[] {simulationComparison}, _container)
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class SimulationComparisonTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableComparison>
   {
      private readonly IContainer _container;

      public SimulationComparisonTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ClassifiableComparison classifiableComparison, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SimulationComparisonContextMenu(classifiableComparison.Comparison, _container);
      }
   }
}