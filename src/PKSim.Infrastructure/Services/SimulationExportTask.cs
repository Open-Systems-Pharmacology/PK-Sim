using System;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

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
      private readonly ISimulationToModelCoreSimulationMapper _coreSimulationMapper;
      private readonly ISimModelExporter _simModelExporter;
      private readonly ISimulationResultsToDataTableConverter _simulationResultsToDataTableConverter;

      public SimulationExportTask(IBuildingBlockTask buildingBlockTask, IDialogCreator dialogCreator, IDataRepositoryTask dataRepositoryTask,
         IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, IStringSerializer stringSerializer,
         IModelReportCreator modelReportCreator, ISimulationToModelCoreSimulationMapper coreSimulationMapper,
         ISimModelExporter simModelExporter, ISimulationResultsToDataTableConverter simulationResultsToDataTableConverter)
      {
         _buildingBlockTask = buildingBlockTask;
         _dialogCreator = dialogCreator;
         _dataRepositoryTask = dataRepositoryTask;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _stringSerializer = stringSerializer;
         _modelReportCreator = modelReportCreator;
         _coreSimulationMapper = coreSimulationMapper;
         _simModelExporter = simModelExporter;
         _simulationResultsToDataTableConverter = simulationResultsToDataTableConverter;
      }

      public Task ExportResultsToExcel(IndividualSimulation individualSimulation)
      {
         _buildingBlockTask.LoadResults(individualSimulation);
         if (!individualSimulation.HasResults)
            throw new PKSimException(PKSimConstants.Error.CannotExportResultsPleaseRunSimulation(individualSimulation.Name));

         return exportToFileAsync(PKSimConstants.UI.ExportSimulationResultsToExcel, Constants.Filter.EXCEL_SAVE_FILE_FILTER, CoreConstants.DefaultResultsExportNameFor(individualSimulation.Name), async fileName =>
         {
            var dataTables = _dataRepositoryTask.ToDataTable(individualSimulation.DataRepository, x => _quantityDisplayPathMapper.DisplayPathAsStringFor(individualSimulation, x), x => x.Dimension);
            await Task.Run(() => _dataRepositoryTask.ExportToExcel(dataTables, fileName, launchExcel: true));
         }, Constants.DirectoryKey.REPORT);
      }

      public Task ExportResultsToCSVAsync(Simulation simulation)
      {
         _buildingBlockTask.LoadResults(simulation);
         if (!simulation.HasResults)
            throw new PKSimException(PKSimConstants.Error.CannotExportResultsPleaseRunSimulation(simulation.Name));

         return exportToFileAsync(PKSimConstants.UI.ExportSimulationResultsToCSV, Constants.Filter.CSV_FILE_FILTER, CoreConstants.DefaultResultsExportNameFor(simulation.Name), async fileName => { await ExportResultsToCSVAsync(simulation, fileName); }, Constants.DirectoryKey.REPORT);
      }

      public async Task ExportResultsToCSVAsync(Simulation simulation, string fileName)
      {
         var dataTable = await _simulationResultsToDataTableConverter.ResultsToDataTable(simulation);
         dataTable.ExportToCSV(fileName);
      }

      public Task ExportSimulationToXmlAsync(Simulation simulation)
      {
         return exportSimulationToFileAsync(PKSimConstants.UI.SaveSimulationToXmlFile, Constants.Filter.XML_FILE_FILTER, simulation, async fileName =>
         {
            using (var writer = new StreamWriter(fileName))
            {
               await writer.WriteAsync(_stringSerializer.Serialize(simulation));
            }
         }, Constants.DirectoryKey.MODEL_PART);
      }

      public Task ExportSimulationToSimModelXmlAsync(Simulation simulation)
      {
         return exportSimulationToFileAsync(PKSimConstants.UI.SaveSimulationToSimModelXmlFile, Constants.Filter.XML_FILE_FILTER, simulation,
            async fileName => await ExportSimulationToSimModelXmlAsync(simulation, fileName), Constants.DirectoryKey.SIM_MODEL_XML);
      }

      public Task ExportSimulationToSimModelXmlAsync(Simulation simulation, string fileName)
      {
         return Task.Run(() => _simModelExporter.Export(_coreSimulationMapper.MapFrom(simulation, shouldCloneModel: false), fileName));
      }

      public Task CreateSimulationReport(Simulation simulation)
      {
         return exportSimulationToFileAsync(PKSimConstants.UI.ExportSimulationModelToFileTitle, Constants.Filter.TEXT_FILE_FILTER, simulation, async exportFile =>
         {
            using (var writer = new StreamWriter(exportFile))
            {
               await writer.WriteAsync(_modelReportCreator.ModelReport(simulation.Model, reportFormulaReferences: true, reportFormulaObjectPaths: false, reportDescriptions: false));
            }

            FileHelper.TryOpenFile(exportFile);
         }, Constants.DirectoryKey.MODEL_PART);
      }

      public Task ExportPKAnalysesToCSVAsync(PopulationSimulation populationSimulation)
      {
         _buildingBlockTask.Load(populationSimulation);
         if (!populationSimulation.HasPKAnalyses)
            throw new PKSimException(PKSimConstants.Error.CannotExportPKAnalysesPleaseRunSimulation(populationSimulation.Name));

         return exportToFileAsync(PKSimConstants.UI.ExportPKAnalysesToCSVTitle, Constants.Filter.CSV_FILE_FILTER, CoreConstants.DefaultPKAnalysesExportNameFor(populationSimulation.Name), async fileName => { await ExportPKAnalysesToCSVAsync(populationSimulation, fileName); }, Constants.DirectoryKey.REPORT);
      }

      public async Task ExportPKAnalysesToCSVAsync(PopulationSimulation populationSimulation, string fileName)
      {
         var dataTable = await _simulationResultsToDataTableConverter.PKAnalysesToDataTable(populationSimulation);
         dataTable.ExportToCSV(fileName);
      }

      private Task exportSimulationToFileAsync(string title, string filter, Simulation simulation, Func<string, Task> actionToPerform, string directoryKey)
      {
         return exportToFileAsync(title, filter, simulation.Name, async fileName =>
         {
            _buildingBlockTask.Load(simulation);
            await actionToPerform(fileName);
         }, directoryKey);
      }

      private async Task exportToFileAsync(string title, string filter, string defaultName, Func<string, Task> actionToPerform, string directoryKey)
      {
         var exportFile = _dialogCreator.AskForFileToSave(title, filter, directoryKey, defaultName);
         if (string.IsNullOrEmpty(exportFile)) return;

         await actionToPerform(exportFile);
      }
   }
}