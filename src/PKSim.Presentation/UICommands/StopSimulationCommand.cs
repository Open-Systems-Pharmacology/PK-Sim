using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
    public class StopSimulationCommand : IUICommand
   {
      private readonly IInteractiveSimulationRunner _simulationRunner;

      public StopSimulationCommand(IInteractiveSimulationRunner simulationRunner)
      {
         _simulationRunner = simulationRunner;
      }

      public void Execute()
      {
         _simulationRunner.StopSimulation();
      }
   }
}