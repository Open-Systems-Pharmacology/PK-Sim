using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportSimulationPopulationToCSVCommand : ContextSpecification<ExportPopulationSimulationToCSVCommand>
   {
      protected IPopulationExportTask _populationExportTask;
      protected IActiveSubjectRetriever _activeSubjectRetriever;
      protected IApplicationController _applicationController;
      protected ISelectFilePresenter _selectFilePresenter;
      protected FileSelection _fileSelection;

      protected override void Context()
      {
         _populationExportTask = A.Fake<IPopulationExportTask>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         _applicationController = A.Fake<IApplicationController>();
         _selectFilePresenter = A.Fake<ISelectFilePresenter>();
         A.CallTo(_applicationController).WithReturnType<ISelectFilePresenter>().Returns(_selectFilePresenter);

         A.CallTo(_selectFilePresenter).WithReturnType<FileSelection>().Returns(_fileSelection);

         sut = new ExportPopulationSimulationToCSVCommand(_populationExportTask, _applicationController, _activeSubjectRetriever);
      }
   }

   public class When_executing_the_export_simulation_population_to_csv_command : concern_for_ExportSimulationPopulationToCSVCommand
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         sut.For(_populationSimulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_export_task_to_export_all_parameters_of_the_given_population_to_the_csv_file()
      {
         A.CallTo(() => _populationExportTask.ExportToCSV(_populationSimulation, _fileSelection)).MustHaveHappened();
      }
   }

   public class When_executing_the_export_simulation_population_to_csv_command_and_the_subject_was_not_set : concern_for_ExportSimulationPopulationToCSVCommand
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _activeSubjectRetriever.Active<PopulationSimulation>()).Returns(_populationSimulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_export_task_to_export_all_parameters_of_the_active_population_to_the_csv_file()
      {
         A.CallTo(() => _populationExportTask.ExportToCSV(_populationSimulation, _fileSelection)).MustHaveHappened();
      }
   }
}