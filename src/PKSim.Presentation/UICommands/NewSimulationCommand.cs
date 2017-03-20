using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewSimulationCommand : AddBuildingBlockUICommand<Simulation, ISimulationTask>
   {
      public NewSimulationCommand(ISimulationTask simulationTask) : base(simulationTask)
      {
      }
   }
}