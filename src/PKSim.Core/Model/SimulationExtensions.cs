using OSPSuite.Core.Domain;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Core.Model
{
   public static class SimulationExtensions
   {
      public static bool IsRunning(this ISimulation simulation, IContainer container)
      {
         if (simulation == null) return false;
         if (container == null) return false;

         var runner = container.Resolve<IInteractiveSimulationRunner>();
         return runner?.IsSimulationRunning(simulation as Simulation) ?? false;
      }

      public static bool IsIdle(this ISimulation simulation, IContainer container)
         => !simulation.IsRunning(container);
   }
}