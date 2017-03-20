using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class RunSimulationCommand : ObjectUICommand<Simulation>
   {
      private readonly ISimulationRunner _simulationRunner;

      public RunSimulationCommand(ISimulationRunner simulationRunner)
      {
         _simulationRunner = simulationRunner;
      }

      protected override void PerformExecute()
      {
         _simulationRunner.RunSimulation(Subject);
      }
   }
}