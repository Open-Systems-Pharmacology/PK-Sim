using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportSimulationToCppUICommand : ContextSpecification<ExportSimulationToCppUICommand>
   {
      protected ISimulationExportTask _simulationExportTask;
      private IActiveSubjectRetriever _activateSubjectRetriever;

      protected override void Context()
      {
         _simulationExportTask = A.Fake<ISimulationExportTask>();
         _activateSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new ExportSimulationToCppUICommand(_simulationExportTask, _activateSubjectRetriever);
      }
   }

   public class When_executing_the_export_simulation_to_cpp_ui_command : concern_for_ExportSimulationToCppUICommand
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();
         sut.For(_simulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_export_to_cpp_utility_to_export_the_simulation()
      {
         A.CallTo(() => _simulationExportTask.ExportSimulationToCppAsync(_simulation)).MustHaveHappened();
      }
   }
}