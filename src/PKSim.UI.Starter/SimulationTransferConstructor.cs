using OSPSuite.Core.Serialization.Exchange;
using PKSim.Core.Services;

namespace PKSim.UI.Starter
{
   public static class SimulationTransferConstructor
   {
      public static object CreateSimulationTransfer(string projectSnapshot)
      {
         var container = ApplicationStartup.Initialize();
         var projectSnapshotToSimulationMapper = container.Resolve<IProjectSnapshotToSimulationMapper>();
         var modelCoreSimulation = projectSnapshotToSimulationMapper.MapFrom(projectSnapshot);
         return new SimulationTransfer
         {
            Simulation = modelCoreSimulation,
         };
      }
   }
}