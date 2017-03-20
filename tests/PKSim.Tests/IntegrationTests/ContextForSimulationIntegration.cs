using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public abstract class ContextForSimulationIntegration<T, TSimulation> : ContextForIntegration<T> where TSimulation : Simulation
   {
      protected TSimulation _simulation;

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         Unregister(_simulation);
      }
   }

   public abstract class ContextForSimulationIntegration<T> : ContextForSimulationIntegration<T, IndividualSimulation>
   {
   }
}