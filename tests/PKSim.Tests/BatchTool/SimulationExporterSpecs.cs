using System.IO;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.SimModel.Services;
using PKSim.BatchTool.Services;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.BatchTool
{
   public abstract class concern_for_SimulationExporter : ContextSpecification<ISimulationExporter>
   {
      private ISimulationEngineFactory _simulationEngineFactory;
      private IBatchLogger _logger;
      protected IParametersReportCreator _parameterReportCreator;
      protected ISimulationResultsExporter _simulationResultsExporter;
      protected IndividualSimulation _simulation;
      protected string _outputFolder;
      protected string _exportName;
      protected SimulationConfiguration _configuration;
      protected BatchExportMode _mode;
      protected ISimulationEngine<IndividualSimulation> _simulationEngine;
      protected ISimulationExportTask _simulationExportTask;

      protected override void Context()
      {
         _simulationEngineFactory = A.Fake<ISimulationEngineFactory>();
         _logger = A.Fake<IBatchLogger>();
         _parameterReportCreator = A.Fake<IParametersReportCreator>();
         _simulationResultsExporter = A.Fake<ISimulationResultsExporter>();
         _simulationExportTask= A.Fake<ISimulationExportTask>();
         sut = new SimulationExporter(_simulationEngineFactory, _logger, _parameterReportCreator, _simulationResultsExporter,_simulationExportTask);

         _simulation = new IndividualSimulation {DataRepository = new DataRepository("Rep")};
         _outputFolder = "OutputFolder";
         _exportName = "ExportName";
         _configuration = new SimulationConfiguration();

         _simulationEngine = A.Fake<ISimulationEngine<IndividualSimulation>>();
         A.CallTo(() => _simulationEngineFactory.Create<IndividualSimulation>()).Returns(_simulationEngine);
      }

      protected override void Because()
      {
         sut.RunAndExport(_simulation, _outputFolder, _exportName, _configuration, _mode);
      }
   }

   public class When_running_and_exporting_a_simulation_for_batch_run_to_csv_and_json_and_xml : concern_for_SimulationExporter
   {
      protected override void Context()
      {
         base.Context();
         _mode = BatchExportMode.Csv | BatchExportMode.Json | BatchExportMode.All;
      }

      [Observation]
      public void should_run_the_simulation()
      {
         A.CallTo(() => _simulationEngine.RunForBatch(_simulation, _configuration.CheckForNegativeValues)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_result_to_json()
      {
         var fileName = Path.Combine(_outputFolder, $"{_exportName}.json");
         A.CallTo(() => _simulationResultsExporter.ExportToJson(_simulation, _simulation.DataRepository, fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_result_to_csv()
      {
         var fileName = Path.Combine(_outputFolder, $"{_exportName}.csv");
         A.CallTo(() => _simulationResultsExporter.ExportToCsv(_simulation, _simulation.DataRepository, fileName)).MustHaveHappened();
      }


      [Observation]
      public void should_export_the_simmodel_simulation_to_xml()
      {
         var fileName = Path.Combine(_outputFolder, "Xml", $"{_exportName}.xml");
         A.CallTo(() => _simulationExportTask.ExportSimulationToSimModelXml(_simulation, fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_parameter_values_to_csv()
      {
         var fileName = Path.Combine(_outputFolder, $"{_exportName}_parameters.csv");
         A.CallTo(() => _parameterReportCreator.ExportParametersTo(_simulation.Model, fileName)).MustHaveHappened();
      }
   }


   public class When_running_and_exporting_a_simulation_for_batch_run_to_csv_only : concern_for_SimulationExporter
   {
      protected override void Context()
      {
         base.Context();
         _mode = BatchExportMode.Csv;
      }

      [Observation]
      public void should_run_the_simulation()
      {
         A.CallTo(() => _simulationEngine.RunForBatch(_simulation, _configuration.CheckForNegativeValues)).MustHaveHappened();
      }

      [Observation]
      public void should_not_export_the_result_to_json()
      {
         A.CallTo(() => _simulationResultsExporter.ExportToJson(_simulation, _simulation.DataRepository, A<string>._)).MustNotHaveHappened();
      }


      [Observation]
      public void should_not_export_the_result_to_xml()
      {
         A.CallTo(() => _simulationExportTask.ExportSimulationToSimModelXml(_simulation,  A<string>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_export_the_result_to_csv()
      {
         A.CallTo(() => _simulationResultsExporter.ExportToCsv(_simulation, _simulation.DataRepository, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_parameter_values_to_csv()
      {
         A.CallTo(() => _parameterReportCreator.ExportParametersTo(_simulation.Model, A<string>._)).MustHaveHappened();
      }
   }

   public class When_running_and_exporting_a_simulation_for_batch_run_to_json_only : concern_for_SimulationExporter
   {
      protected override void Context()
      {
         base.Context();
         _mode = BatchExportMode.Json;
      }

      [Observation]
      public void should_run_the_simulation()
      {
         A.CallTo(() => _simulationEngine.RunForBatch(_simulation, _configuration.CheckForNegativeValues)).MustHaveHappened();
      }

      [Observation]
      public void should_nexport_the_result_to_json()
      {
         A.CallTo(() => _simulationResultsExporter.ExportToJson(_simulation, _simulation.DataRepository, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_not_export_the_result_to_csv()
      {
         A.CallTo(() => _simulationResultsExporter.ExportToCsv(_simulation, _simulation.DataRepository, A<string>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_export_the_parameter_values_to_csv()
      {
         A.CallTo(() => _parameterReportCreator.ExportParametersTo(_simulation.Model, A<string>._)).MustNotHaveHappened();
      }
   }
}