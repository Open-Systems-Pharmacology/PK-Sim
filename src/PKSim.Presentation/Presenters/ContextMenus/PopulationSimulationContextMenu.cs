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
   public class PopulationSimulationContextMenu : SimulationContextMenu<PopulationSimulation>
   {
      public PopulationSimulationContextMenu(PopulationSimulation populationSimulation, IContainer container)
         : base(populationSimulation, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> ExportMenuItemsSpecificToType(PopulationSimulation populationSimulation)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadPopulationAnalysisWorkflowFromTemplate)
            .WithCommandFor<LoadPopulationAnalysisWorkflowFromTemplateUICommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.LoadFromTemplate)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SavePopulationAnalysisWorkflowToTemplate)
            .WithCommandFor<SavePopulationAnalysisWorkflowToTemplateUICommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.SaveAsTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPopulationToCSV)
            .WithCommandFor<ExportPopulationSimulationToCSVCommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.PopulationExportToCSV)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSV)
            .WithCommandFor<ExportSimulationResultsToCSVCommand, Simulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.ExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPKAnalysesToCSV)
            .WithCommandFor<ExportPopulationSimulationPKAnalysesCommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.PKAnalysesExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportForClusterComputations)
            .WithCommandFor<ExportPopulationSimulationForClusterCommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.ClusterExport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportSimulationResultsFromCSV)
            .WithCommandFor<ImportSimulationResultsCommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.ResultsImportFromCSV)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPKAnalysesFromCSV)
            .WithCommandFor<ImportPKAnalysesCommand, PopulationSimulation>(populationSimulation, _container)
            .WithIcon(ApplicationIcons.PKAnalysesImportFromCSV);
      }

      protected override IEnumerable<IMenuBarItem> DebugMenuFor(PopulationSimulation simulation)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Save Snapshot"))
            .WithCommandFor<ExportSimulationSnapshotUICommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.SnapshotExport)
            .ForDeveloper();
      }

  
   }

   public class PopulationSimulationTreeNodeContextMenuFactory : SimulationTreeNodeContextMenuFactory<PopulationSimulation>
   {
      private readonly IContainer _container;

      public PopulationSimulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(PopulationSimulation simulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new PopulationSimulationContextMenu(simulation, _container);
      }
   }
}