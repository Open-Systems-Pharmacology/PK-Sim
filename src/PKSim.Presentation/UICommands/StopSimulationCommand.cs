using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
    public class StopSimulationCommand : IUICommand
   {
      private readonly ISimulationRunner _simulationRunner;

      public StopSimulationCommand(ISimulationRunner simulationRunner)
      {
         _simulationRunner = simulationRunner;
      }

      public void Execute()
      {
         _simulationRunner.StopSimulation();
      }
   }
}