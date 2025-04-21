using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Services;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.Presentation.UICommands
{
   public class StopSimulationCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IInteractiveSimulationRunner _simulationRunner;

      public StopSimulationCommand(IInteractiveSimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationRunner = simulationRunner;
      }

      protected override async void PerformExecute()
      {
         _simulationRunner.StopSimulation(Subject);
      }
   }
}