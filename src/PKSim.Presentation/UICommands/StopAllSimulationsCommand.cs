using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class StopAllSimulationsCommand : IUICommand
   {
      private readonly IInteractiveSimulationRunner _interactiveSimulationRunnerTask;

      public StopAllSimulationsCommand(IInteractiveSimulationRunner interactiveSimulationRunnerTask)
      {
         _interactiveSimulationRunnerTask = interactiveSimulationRunnerTask;
      }

      public void Execute()
      {
         _interactiveSimulationRunnerTask.StopAllSimulations();
      }
   }
}
