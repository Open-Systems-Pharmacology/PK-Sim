using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_RunSimulationQualificationStepRunner : ContextSpecificationAsync<RunSimulationQualificationStepRunner>
   {
      protected ISimulationRunner _simulationRunner;
      private ILogger _logger;
      protected RunSimulationQualificationStep _runSimulationQualificationStep;
      protected Simulation _simulation;

      protected override Task Context()
      {
         _simulationRunner= A.Fake<ISimulationRunner>();
         _logger= A.Fake<ILogger>();   
         sut = new RunSimulationQualificationStepRunner(_simulationRunner,_logger);

         _simulation = new IndividualSimulation();
         _runSimulationQualificationStep = new RunSimulationQualificationStep {Simulation = _simulation};
         return _completed;
      }
   }

   public class When_run_the_run_simulation_qualification_step_runner : concern_for_RunSimulationQualificationStepRunner
   {

      protected override async Task Because()
      {
         await sut.RunAsync(_runSimulationQualificationStep);
      }

      [Observation]
      public void should_leverage_the_simulation_runner_to_run_the_simulation()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(_simulation, null)).MustHaveHappened();
      }
   }
}	