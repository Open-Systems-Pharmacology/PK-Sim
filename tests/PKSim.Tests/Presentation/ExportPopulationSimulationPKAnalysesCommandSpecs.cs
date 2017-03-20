using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportPopulationSimulationPKAnalysesCommandv : ContextSpecification<ExportPopulationSimulationPKAnalysesCommand>
   {
      protected ISimulationExportTask _simulationTask;
      private IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationTask = A.Fake<ISimulationExportTask>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new ExportPopulationSimulationPKAnalysesCommand(_simulationTask, _activeSubjectRetriever);
      }
   }

   public class When_executing_the_export_pk_analyses_command : concern_for_ExportPopulationSimulationPKAnalysesCommandv
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         sut.Subject = _populationSimulation;
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_export_task_to_export_the_pk_analyses_to_file()
      {
         A.CallTo(() => _simulationTask.ExportPKAnalysesToCSVAsync(_populationSimulation)).MustHaveHappened();
      }
   }
}