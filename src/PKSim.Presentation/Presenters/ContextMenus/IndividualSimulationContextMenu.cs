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
   public class IndividualSimulationContextMenu : SimulationContextMenu<IndividualSimulation>
   {
      public IndividualSimulationContextMenu(IndividualSimulation individualSimulation) : base(individualSimulation)
      {
      }

      protected override IEnumerable<IMenuBarItem> ActionMenuItemsSpecificToType(IndividualSimulation individualSimulation)
      {
         yield return CreateParameterIdentificationMenuItemFor(individualSimulation);
         yield return CreateSensitivityAnalysisMenuItemFor(individualSimulation);
      }

      protected override IEnumerable<IMenuBarItem> ExportMenuItemsSpecificToType(IndividualSimulation individualSimulation)
      {
         var exportToExcel = CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToExcel)
            .WithCommandFor<ExportSimulationResultsToExcelCommand, IndividualSimulation>(individualSimulation)
            .WithIcon(ApplicationIcons.ExportToExcel);

         exportToExcel.Enabled = simulationHasResult(individualSimulation);
         yield return exportToExcel;

         var exportToCSV = CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSV)
            .WithCommandFor<ExportSimulationResultsToCSVCommand, Simulation>(individualSimulation)
            .WithIcon(ApplicationIcons.ExportToCSV);

         exportToCSV.Enabled = simulationHasResult(individualSimulation);
         yield return exportToCSV;

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationModelToFile)
            .WithIcon(ApplicationIcons.Report)
            .WithCommandFor<CreateSimulationReportCommand, Simulation>(individualSimulation);
      }

      protected override IEnumerable<IMenuBarItem> DebugMenuFor(IndividualSimulation simulation)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.ExportSimModelXml)
            .WithCommandFor<ExportSimulationToSimModelXmlUICommand, Simulation>(simulation)
            .AsGroupStarter()
            .ForDeveloper();

         yield return ExportSimulationToCppMenuItem(simulation);
         yield return ExportODEForMatlabMenuItem(simulation);
         yield return ExportODEForRMenuItem(simulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Parameter Ids Export"))
            .WithCommandFor<ParameterExportForDebugCommand, Simulation>(simulation)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Simulation Serialization Xml Export"))
            .WithCommandFor<SimulationXmlExportCommand, Simulation>(simulation)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Simulation Parameter Export To CSV"))
            .WithCommandFor<SimulationParameterExportToCsvCommand, Simulation>(simulation)
            .ForDeveloper();


         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Save Snapshot"))
            .WithCommandFor<ExportSimulationSnapshotUICommand, Simulation>(simulation)
            .WithIcon(ApplicationIcons.SnapshotExport)
            .ForDeveloper();

      }

      protected IMenuBarItem CreateParameterIdentificationMenuItemFor(IndividualSimulation simulation)
      {
         return ParameterIdentificationContextMenuItems.CreateParameterIdentificationFor(new[] { simulation });
      }

      protected IMenuBarItem CreateSensitivityAnalysisMenuItemFor(IndividualSimulation simulation)
      {
         return SensitivityAnalysisContextMenuItems.CreateSensitivityAnalysisFor(simulation);
      }

      private bool simulationHasResult(Simulation simulation)
      {
         //can't tell for sure, since the simulation is not loaded. Return true
         if (!simulation.IsLoaded) return true;

         return simulation.HasResults;
      }
   }

   public class IndividualSimulationTreeNodeContextMenuFactory : SimulationTreeNodeContextMenuFactory<IndividualSimulation>
   {
      protected override IContextMenu CreateFor(IndividualSimulation simulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualSimulationContextMenu(simulation);
      }
   }
}