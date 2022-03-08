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

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationComparisonContextMenu : ContextMenu<ISimulationComparison>
   {
      public SimulationComparisonContextMenu(ISimulationComparison summaryChart) : base(summaryChart)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ISimulationComparison simulationComparison)
      {
         yield return GenericMenu.EditMenuFor<EditSubjectUICommand<ISimulationComparison>, ISimulationComparison>(simulationComparison);

         yield return GenericMenu.RenameMenuFor(simulationComparison);

         yield return GenericMenu.EditDescriptionMenuFor(simulationComparison)
            .AsGroupStarter();

         var populationSimulationComparison = simulationComparison as PopulationSimulationComparison;
         if (populationSimulationComparison != null)
         {
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Configure)
               .WithCommandFor<ConfigurePopulationSimulationComparison, PopulationSimulationComparison>(populationSimulationComparison)
               .AsGroupStarter();
         }

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<DeleteSimulationComparisonsUICommand, IReadOnlyList<ISimulationComparison>>(new[] {simulationComparison})
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class SimulationComparisonTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableComparison>
   {
      public override IContextMenu CreateFor(ClassifiableComparison classifiableComparison, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SimulationComparisonContextMenu(classifiableComparison.Comparison);
      }
   }
}