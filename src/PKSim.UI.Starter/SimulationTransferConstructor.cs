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
         var moBiExportTask = container.Resolve<IMoBiExportTask>();
         var modelCoreSimulation = projectSnapshotToSimulationMapper.MapFrom(projectSnapshot);
         var transfer = new SimulationTransfer
         {
            Simulation = modelCoreSimulation,
         };

         return moBiExportTask.Serialize(transfer);
      }
   }
}