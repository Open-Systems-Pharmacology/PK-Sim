using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportSimulationPopulationToCSVCommand : ContextSpecification<ExportPopulationSimulationToCSVCommand>
   {
      protected IPopulationExportTask _populationExportTask;
      protected IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _populationExportTask = A.Fake<IPopulationExportTask>();
         _activeSubjectRetriever= A.Fake<IActiveSubjectRetriever>();
         sut = new ExportPopulationSimulationToCSVCommand(_populationExportTask,_activeSubjectRetriever);
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
         A.CallTo(() => _populationExportTask.ExportToCSV(_populationSimulation)).MustHaveHappened();
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
         A.CallTo(() => _populationExportTask.ExportToCSV(_populationSimulation)).MustHaveHappened();
      }
   }
}