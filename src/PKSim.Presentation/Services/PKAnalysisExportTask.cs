using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
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
      private readonly IDimensionFactory _dimensionFactory;

      public PKAnalysisExportTask(IDialogCreator dialogCreator, IDataRepositoryTask dataRepositoryTask, IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper,
         IGlobalPKAnalysisToDataTableMapper globalPKAnalysisToDataTableMapper, IDimensionFactory dimensionFactory)
      {
         _dialogCreator = dialogCreator;
         _dataRepositoryTask = dataRepositoryTask;
         _globalPKAnalysisToDataTableMapper = globalPKAnalysisToDataTableMapper;
         _dimensionFactory = dimensionFactory;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
      }

      public void ExportToExcel(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable pkAnalysisDataTable, IEnumerable<Simulation> simulations)
      {
         var allSimulations = simulations.ToList();
         string defaultFileName = allSimulations.Count == 1 ? allSimulations[0].Name : string.Empty;
         var fileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportPKAnalysesToExcelTitle, Constants.Filter.EXCEL_SAVE_FILE_FILTER, Constants.DirectoryKey.REPORT, defaultFileName);
         if (string.IsNullOrEmpty(fileName)) return;

         var allDataTables = ExportToDataTable(dataColumns, globalPKAnalysis, pkAnalysisDataTable, allSimulations);
         _dataRepositoryTask.ExportToExcel(allDataTables, fileName, launchExcel:true);
      }

      public IEnumerable<DataTable> ExportToDataTable(IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis, DataTable dataTable, IEnumerable<Simulation> simulations)
      {
         var allDataTables = new List<DataTable>();
         var allSimulations = simulations.ToList();

         if (allSimulations.Any())
         {
            string ColumnNameRetriever(DataColumn dataColumn) => _quantityDisplayPathMapper.DisplayPathAsStringFor(simulationForColumn(dataColumn, allSimulations), dataColumn);
            allDataTables.AddRange(_dataRepositoryTask.ToDataTable(dataColumns, ColumnNameRetriever, _dimensionFactory.MergedDimensionFor));
         }

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