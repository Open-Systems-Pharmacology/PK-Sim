using System.Collections.Generic;
using System.Data;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Mappers;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Presentation.Services
{
   public interface IPKAnalysisExportTask
   {
      void ExportToExcel(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable pkAnalysisDataTable, IEnumerable<Simulation> simulations);
      IEnumerable<DataTable> ExportToDataTable(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable dataTable, IEnumerable<Simulation> simulations);
      void ExportToExcel(DataTable dataTable, string defaultName);

   }

   public class PKAnalysisExportTask : IPKAnalysisExportTask
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;

      private readonly IGlobalPKAnalysisToDataTableMapper _globalPKAnalysisToDataTableMapper;

      public PKAnalysisExportTask(IDialogCreator dialogCreator, IDataRepositoryTask dataRepositoryTask, IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper,
         IGlobalPKAnalysisToDataTableMapper globalPKAnalysisToDataTableMapper)
      {
         _dialogCreator = dialogCreator;
         _dataRepositoryTask = dataRepositoryTask;
         _globalPKAnalysisToDataTableMapper = globalPKAnalysisToDataTableMapper;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
      }

      public void ExportToExcel(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable pkAnalysisDataTable, IEnumerable<Simulation> simulations)
      {
         var allSimulations = simulations.ToList();
         string defaultFileName = allSimulations.Count == 1 ? allSimulations[0].Name : string.Empty;
         var fileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportPKAnalysesToExcelTitle, Constants.Filter.EXCEL_SAVE_FILE_FILTER, Constants.DirectoryKey.REPORT, defaultFileName);
         if (string.IsNullOrEmpty(fileName)) return;

         var allDataTables = ExportToDataTable(dataColumns, globalPKAnalysis, pkAnalysisDataTable, allSimulations);
         _dataRepositoryTask.ExportToExcel(allDataTables, fileName, true);
      }

      public IEnumerable<DataTable> ExportToDataTable(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable dataTable, IEnumerable<Simulation> simulations)
      {
         var allDataTables = new List<DataTable>();
         var allSimulations = simulations.ToList();
         if (allSimulations.Any())
            allDataTables.AddRange(_dataRepositoryTask.ToDataTable(dataColumns, x => _quantityDisplayPathMapper.DisplayPathAsStringFor(simulationForColumn(x, allSimulations), x)));

         allDataTables.Add(_globalPKAnalysisToDataTableMapper.MapFrom(globalPKAnalysis));
         allDataTables.Add(dataTable);

         return allDataTables;
      }

      public void ExportToExcel(DataTable dataTable, string defaultName)
      {
         var fileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportPKAnalysesToExcelTitle, Constants.Filter.EXCEL_SAVE_FILE_FILTER, Constants.DirectoryKey.REPORT, defaultName);
         if (string.IsNullOrEmpty(fileName)) return;

         _dataRepositoryTask.ExportToExcel(new[] { dataTable }, fileName, true);
      }


      private Simulation simulationForColumn(DataColumn dataColumn, IEnumerable<Simulation> allSimulations)
      {
         return dataColumn.ParentSimulationIn(allSimulations);
      }
   }
}