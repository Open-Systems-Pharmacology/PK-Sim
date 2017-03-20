using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CloneSimulationCommand : ContextSpecification<CloneSimulationCommand>
   {
      protected ICloneSimulationTask _simulationTask;
      private IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationTask = A.Fake<ICloneSimulationTask>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new CloneSimulationCommand(_simulationTask, _activeSubjectRetriever);
      }
   }

   public class When_executing_the_clone_simulation_command : concern_for_CloneSimulationCommand
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         sut.For(_simulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_start_the_clone_action_in_the_simulation_task()
      {
         A.CallTo(() => _simulationTask.Clone(_simulation)).MustHaveHappened();
      }
   }
}