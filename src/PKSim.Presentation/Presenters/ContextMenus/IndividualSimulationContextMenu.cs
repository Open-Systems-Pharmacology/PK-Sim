using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class IndividualSimulationContextMenu : SimulationContextMenu<IndividualSimulation>
   {
      public IndividualSimulationContextMenu(IndividualSimulation individualSimulation, IContainer container) : base(individualSimulation, container)
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
            .WithCommandFor<ExportSimulationResultsToExcelCommand, IndividualSimulation>(individualSimulation, _container)
            .WithIcon(ApplicationIcons.ExportToExcel);

         exportToExcel.Enabled = simulationHasResult(individualSimulation);
         yield return exportToExcel;

         var exportToCSV = CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSV)
            .WithCommandFor<ExportSimulationResultsToCSVCommand, Simulation>(individualSimulation, _container)
            .WithIcon(ApplicationIcons.ExportToCSV);

         exportToCSV.Enabled = simulationHasResult(individualSimulation);
         yield return exportToCSV;

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationModelToFile)
            .WithIcon(ApplicationIcons.Report)
            .WithCommandFor<CreateSimulationReportCommand, Simulation>(individualSimulation, _container);
      }

      protected override IEnumerable<IMenuBarItem> DebugMenuFor(IndividualSimulation simulation)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.ExportSimModelXml)
            .WithCommandFor<ExportSimulationToSimModelXmlUICommand, Simulation>(simulation, _container)
            .AsGroupStarter()
            .ForDeveloper();

         yield return ExportSimulationToCppMenuItem(simulation);
         yield return ExportODEForMatlabMenuItem(simulation);
         yield return ExportODEForRMenuItem(simulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Parameter Ids Export"))
            .WithCommandFor<ParameterExportForDebugCommand, Simulation>(simulation, _container)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Simulation Serialization Xml Export"))
            .WithCommandFor<SimulationXmlExportCommand, Simulation>(simulation, _container)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Simulation Parameter Export To CSV"))
            .WithCommandFor<SimulationParameterExportToCsvCommand, Simulation>(simulation, _container)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Save Snapshot"))
            .WithCommandFor<ExportSimulationSnapshotUICommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.SnapshotExport)
            .ForDeveloper();

         yield return ExportMarkdownMenuFor(simulation);
      }

      protected IMenuBarItem CreateParameterIdentificationMenuItemFor(IndividualSimulation simulation)
      {
         return ParameterIdentificationContextMenuItems.CreateParameterIdentificationFor(new[] {simulation}, _container);
      }

      protected IMenuBarItem CreateSensitivityAnalysisMenuItemFor(IndividualSimulation simulation)
      {
         return SensitivityAnalysisContextMenuItems.CreateSensitivityAnalysisFor(simulation, _container);
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
      private readonly IContainer _container;

      public IndividualSimulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(IndividualSimulation simulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualSimulationContextMenu(simulation, _container);
      }
   }
}