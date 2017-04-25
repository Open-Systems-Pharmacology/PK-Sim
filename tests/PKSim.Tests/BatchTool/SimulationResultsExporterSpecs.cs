using System.Collections.Generic;
using System.Data;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility;
using PKSim.BatchTool.Mappers;
using PKSim.BatchTool.Services;

namespace PKSim.BatchTool
{
   public abstract class concern_for_SimulationResultsExporter : ContextSpecification<ISimulationResultsExporter>
   {
      protected IDataRepositoryTask _dataRepositoryTask;
      private IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      protected ISimulationResultsToBatchSimulationExportMapper _batchSimulationExportMapper;
      protected ISimulation _simulation;
      protected DataRepository _results;
      protected string _fileName;

      protected override void Context()
      {
         _dataRepositoryTask = A.Fake<IDataRepositoryTask>();
         _quantityDisplayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _batchSimulationExportMapper = A.Fake<ISimulationResultsToBatchSimulationExportMapper>();
         sut = new SimulationResultsExporter(_dataRepositoryTask, _quantityDisplayPathMapper, _batchSimulationExportMapper);

         _simulation = A.Fake<ISimulation>();
         _results = new DataRepository();
         _fileName = FileHelper.GenerateTemporaryFileName();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }

   public class When_exporting_simulation_results_to_csv : concern_for_SimulationResultsExporter
   {
      private List<DataTable> _dataTables;

      protected override void Context()
      {
         base.Context();
         var dataTable = new DataTable("TEST");

         _dataTables = new List<DataTable> {dataTable};
         A.CallTo(_dataRepositoryTask).WithReturnType<IEnumerable<DataTable>>().Returns(_dataTables);
      }
      protected override void Because()
      {
         sut.ExportToCsv(_simulation, _results, _fileName);
      }

      [Observation]
      public void should_create_a_data_table_with_all_results_from_the_simulation_and_export_it_to_the_file()
      {
         FileHelper.FileExists(_fileName).ShouldBeTrue();
      }
   }

   public class When_exporting_simulation_results_to_json : concern_for_SimulationResultsExporter
   {
      private BatchSimulationExport _batchSimulationExport;

      protected override void Context()
      {
         base.Context();
         _batchSimulationExport = new BatchSimulationExport {Name = "Sim"};
         A.CallTo(() => _batchSimulationExportMapper.MapFrom(_simulation, _results)).Returns(_batchSimulationExport);
      }
      protected override void Because()
      {
         sut.ExportToJson(_simulation, _results, _fileName);
      }

      [Observation]
      public void should_create_a_batch_simulation_export_object_and_export_it_to_the_file()
      {
         FileHelper.FileExists(_fileName).ShouldBeTrue();
      }
   }
}