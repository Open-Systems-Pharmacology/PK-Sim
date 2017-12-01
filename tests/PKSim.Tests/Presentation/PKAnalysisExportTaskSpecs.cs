using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using DataColumnn = OSPSuite.Core.Domain.Data.DataColumn;
using IPKAnalysesTask = PKSim.Core.Services.IPKAnalysesTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_PKAnalysisExportTask : ContextSpecification<IPKAnalysisExportTask>
   {
      protected IDataRepositoryTask _dataRepositoryTask;
      protected IDialogCreator _dialogCreator;
      protected IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      protected IPKAnalysesTask _pkAnalysesTask;
      private IGlobalPKAnalysisToDataTableMapper _globalPKAnalysisMapper;
      private IDimensionFactory _dimensionFactory;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _dataRepositoryTask = A.Fake<IDataRepositoryTask>();
         _quantityDisplayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _pkAnalysesTask = A.Fake<IPKAnalysesTask>();
         _globalPKAnalysisMapper = A.Fake<IGlobalPKAnalysisToDataTableMapper>();
         _dimensionFactory= A.Fake<IDimensionFactory>();

         sut = new PKAnalysisExportTask(_dialogCreator, _dataRepositoryTask, _quantityDisplayPathMapper, _globalPKAnalysisMapper, _dimensionFactory);
      }
   }

   public class When_exporting_the_pk_analyses_to_excel_and_the_user_cancels_the_action : concern_for_PKAnalysisExportTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.ExportToExcel(Enumerable.Empty<DataColumnn>(), new GlobalPKAnalysis(), new DataTable(), A.CollectionOfFake<Simulation>(1));
      }

      [Observation]
      public void should_not_export_anything()
      {
         A.CallTo(_dataRepositoryTask).MustNotHaveHappened();
      }
   }

   public class When_exporting_the_pk_analyses_to_excel : concern_for_PKAnalysisExportTask
   {
      private string _fileName;
      private DataColumnn _col1;
      private DataColumnn _col2;
      private GlobalPKAnalysis _global;
      private Simulation _sim1;
      private IEnumerable<DataTable> _dataTables;
      private DataTable _dataTable1;
      private DataTable _dataTable2;

      protected override void Context()
      {
         base.Context();
         _fileName = "file";
         _dataTable1 = new DataTable();
         _dataTable2 = new DataTable();
         var baseGrid = new BaseGrid("time", A.Fake<IDimension>());
         _col1 = new DataColumnn("col1", A.Fake<IDimension>(), baseGrid);
         _col2 = new DataColumnn("col2", A.Fake<IDimension>(), baseGrid);
         _global = new GlobalPKAnalysis();

         _sim1 = A.Fake<Simulation>().WithName("Sim");
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileName);

         A.CallTo(_dataRepositoryTask).WithReturnType<IEnumerable<DataTable>>().Returns(new[] {_dataTable1, _dataTable2});
         A.CallTo(() => _dataRepositoryTask.ExportToExcel(A<IEnumerable<DataTable>>._, _fileName, true))
            .Invokes(x => _dataTables = x.GetArgument<IEnumerable<DataTable>>(0));
      }

      protected override void Because()
      {
         sut.ExportToExcel(new[] {_col1, _col2}, _global, new DataTable(), new[] {_sim1});
      }

      [Observation]
      public void should_have_asked_the_user_to_select_the_file_where_the_data_should_be_exported()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportPKAnalysesToExcelTitle, Constants.Filter.EXCEL_SAVE_FILE_FILTER, Constants.DirectoryKey.REPORT, "Sim", null)).MustHaveHappened();
      }

      [Observation]
      public void should_have_exported_the_results_to_excel()
      {
         A.CallTo(() => _dataRepositoryTask.ExportToExcel(A<IEnumerable<DataTable>>._, _fileName, true)).MustHaveHappened();
      }

      [Observation]
      public void should_have_exported_the_results_and_the_pk_analyses()
      {
         _dataTables.ShouldContain(_dataTable1, _dataTable2);
         _dataTables.Count().ShouldBeEqualTo(4); //2 from ToDataTable +1 for global +1 for local
      }
   }
}