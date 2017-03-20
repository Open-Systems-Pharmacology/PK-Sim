using System;
using System.IO;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Infrastructure.Services
{
   public class SimulationExportTask : ISimulationExportTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      private readonly IStringSerializer _stringSerializer;
      private readonly IModelReportCreator _modelReportCreator;
      private readonly ISimulationToModelCoreSimulationMapper _simulationMapper;
      private readonly ISimModelExporter _simModelExporter;
      private readonly ISimulationResultsToDataTableConverter _simulationResultsToDataTableConverter;

      public SimulationExportTask(IBuildingBlockTask buildingBlockTask, IDialogCreator dialogCreator, IDataRepositoryTask dataRepositoryTask,
         IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, IStringSerializer stringSerializer,
         IModelReportCreator modelReportCreator, ISimulationToModelCoreSimulationMapper simulationMapper, ISimModelExporter simModelExporter, ISimulationResultsToDataTableConverter simulationResultsToDataTableConverter)
      {
         _buildingBlockTask = buildingBlockTask;
         _dialogCreator = dialogCreator;
         _dataRepositoryTask = dataRepositoryTask;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _stringSerializer = stringSerializer;
         _modelReportCreator = modelReportCreator;
         _simulationMapper = simulationMapper;
         _simModelExporter = simModelExporter;
         _simulationResultsToDataTableConverter = simulationResultsToDataTableConverter;
      }

      public void ExportResultsToExcel(IndividualSimulation individualSimulation)
      {
         _buildingBlockTask.LoadResults(individualSimulation);
         if (!individualSimulation.HasResults)
            throw new PKSimException(PKSimConstants.Error.CannotExportResultsPleaseRunSimulation(individualSimulation.Name));

         exportToFile(PKSimConstants.UI.ExportSimulationResultsToExcel, Constants.Filter.EXCEL_SAVE_FILE_FILTER, PKSimConstants.UI.DefaultResultsExportNameFor(individualSimulation.Name), fileName =>
         {
            var dataTables = _dataRepositoryTask.ToDataTable(individualSimulation.DataRepository, x => _quantityDisplayPathMapper.DisplayPathAsStringFor(individualSimulation, x));
            _dataRepositoryTask.ExportToExcel(dataTables, fileName, launchExcel: true);
         }, Constants.DirectoryKey.REPORT);
      }

      public async Task ExportResultsToCSVAsync(Simulation simulation)
      {
         _buildingBlockTask.LoadResults(simulation);
         if (!simulation.HasResults)
            throw new PKSimException(PKSimConstants.Error.CannotExportResultsPleaseRunSimulation(simulation.Name));

         await exportToFileAsync(PKSimConstants.UI.ExportSimulationResultsToCSV, Constants.Filter.CSV_FILE_FILTER, PKSimConstants.UI.DefaultResultsExportNameFor(simulation.Name), async fileName =>
         {
            await ExportResultsToCSVAsync(simulation, fileName);
         }, Constants.DirectoryKey.REPORT);
      }

      public async Task ExportResultsToCSVAsync(Simulation simulation, string fileName)
      {
         var dataTable = await _simulationResultsToDataTableConverter.ResultsToDataTable(simulation);
         dataTable.ExportToCSV(fileName);
      }

      public void ExportSimulationToXml(Simulation simulation)
      {
         exportSimulationToFile(PKSimConstants.UI.SaveSimulationToXmlFile, Constants.Filter.XML_FILE_FILTER, simulation, fileName =>
         {
            using (var writer = new StreamWriter(fileName))
            {
               writer.Write(_stringSerializer.Serialize(simulation));
            }
         }, Constants.DirectoryKey.MODEL_PART);
      }

      public void ExportSimulationToSimModelXml(Simulation simulation)
      {
         exportSimulationToFile(PKSimConstants.UI.SaveSimulationToSimModelXmlFile, Constants.Filter.XML_FILE_FILTER, simulation,
            fileName => _simModelExporter.Export(_simulationMapper.MapFrom(simulation, shouldCloneModel: false), fileName),
            Constants.DirectoryKey.SIM_MODEL_XML);
      }

      public void CreateSimulationReport(Simulation simulation)
      {
         exportSimulationToFile(PKSimConstants.UI.ExportSimulationModelToFileTitle, Constants.Filter.TEXT_FILE_FILTER, simulation, exportFile =>
         {
            using (var writer = new StreamWriter(exportFile))
            {
               writer.Write(_modelReportCreator.ModelReport(simulation.Model, reportFormulaReferences: true, reportFormulaObjectPaths: false, reportDescriptions: false));
            }

            FileHelper.TryOpenFile(exportFile);
         }, Constants.DirectoryKey.MODEL_PART);
      }

      public async Task ExportPKAnalysesToCSVAsync(PopulationSimulation populationSimulation)
      {
         _buildingBlockTask.Load(populationSimulation);
         if (!populationSimulation.HasPKAnalyses)
            throw new PKSimException(PKSimConstants.Error.CannotExportPKAnalysesPleaseRunSimulation(populationSimulation.Name));

         await exportToFileAsync(PKSimConstants.UI.ExportPKAnalysesToCSVTitle, Constants.Filter.CSV_FILE_FILTER, PKSimConstants.UI.DefaultPKAnalysesExportNameFor(populationSimulation.Name), async fileName =>
         {
            var dataTable = await _simulationResultsToDataTableConverter.PKAnalysesToDataTable(populationSimulation);
            dataTable.ExportToCSV(fileName);
         }, Constants.DirectoryKey.REPORT);
      }

      private void exportSimulationToFile(string title, string filter, Simulation simulation, Action<string> actionToPerform, string directoryKey)
      {
         exportToFile(title, filter, simulation.Name, fileName =>
         {
            _buildingBlockTask.Load(simulation);
            actionToPerform(fileName);
         }, directoryKey);
      }

      private void exportToFile(string title, string filter, string defaultName, Action<string> actionToPerform, string directoryKey)
      {
         var exportFile = _dialogCreator.AskForFileToSave(title, filter, directoryKey, defaultName);
         if (string.IsNullOrEmpty(exportFile)) return;

         actionToPerform(exportFile);
      }

      private async Task exportToFileAsync(string title, string filter, string defaultName, Func<string, Task> actionToPerform, string directoryKey)
      {
         var exportFile = _dialogCreator.AskForFileToSave(title, filter, directoryKey, defaultName);
         if (string.IsNullOrEmpty(exportFile)) return;

         await actionToPerform(exportFile);
      }
   }
}