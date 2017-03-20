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
   public class PopulationSimulationContextMenu : SimulationContextMenu<PopulationSimulation>
   {
      public PopulationSimulationContextMenu(PopulationSimulation populationSimulation)
         : base(populationSimulation)
      {
      }

      protected override IEnumerable<IMenuBarItem> ExportMenuItemsSpecificToType(PopulationSimulation populationSimulation)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadPopulationAnalysisWorkflowFromTemplate)
            .WithCommandFor<LoadPopulationAnalysisWorkflowFromTemplateUICommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.LoadFromTemplate)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SavePopulationAnalysisWorkflowToTemplate)
            .WithCommandFor<SavePopulationAnalysisWorkflowToTemplateUICommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.SaveAsTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPopulationToCSV)
            .WithCommandFor<ExportPopulationSimulationToCSVCommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.PopulationExportToCSV)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSV)
            .WithCommandFor<ExportSimulationResultsToCSVCommand, Simulation>(populationSimulation)
            .WithIcon(ApplicationIcons.ExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPKAnalysesToCSV)
            .WithCommandFor<ExportPopulationSimulationPKAnalysesCommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.PKAnalysesExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportForClusterComputations)
            .WithCommandFor<ExportPopulationSimulationForClusterCommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.ClusterExport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportSimulationResultsFromCSV)
            .WithCommandFor<ImportSimulationResultsCommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.ResultsImportFromCSV)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPKAnalysesFromCSV)
            .WithCommandFor<ImportPKAnalysesCommand, PopulationSimulation>(populationSimulation)
            .WithIcon(ApplicationIcons.PKAnalysesImportFromCSV);
      }

      protected override IEnumerable<IMenuBarItem> ActionMenuItemsSpecificToType(PopulationSimulation populationSimulation)
      {
         yield break;
      }
   }

   public class PopulationSimulationTreeNodeContextMenuFactory : SimulationTreeNodeContextMenuFactory<PopulationSimulation>
   {
      protected override IContextMenu CreateFor(PopulationSimulation simulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new PopulationSimulationContextMenu(simulation);
      }
   }
}