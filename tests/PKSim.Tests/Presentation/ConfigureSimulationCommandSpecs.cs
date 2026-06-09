using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ConfigureSimulationCommand : ContextSpecification<ConfigureSimulationCommand>
   {
      protected IConfigureSimulationTask _simulationTask;
      protected Simulation _simulation;
      private IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationTask = A.Fake<IConfigureSimulationTask>();
         _activeSubjectRetriever= A.Fake<IActiveSubjectRetriever>();
         _simulation = A.Fake<Simulation>();
         sut = new ConfigureSimulationCommand(_simulationTask, _activeSubjectRetriever);
         sut.For(_simulation);
      }
   }

   public class When_the_configure_simulation_command_is_executing : concern_for_ConfigureSimulationCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_task_and_configure_the_simulation()
      {
         A.CallTo(() => _simulationTask.Configure(_simulation)).MustHaveHappened();
      }
   }
}