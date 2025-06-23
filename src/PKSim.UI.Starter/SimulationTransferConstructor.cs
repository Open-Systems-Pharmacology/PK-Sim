using PKSim.Core.Services;

namespace PKSim.UI.Starter
{
   public static class SimulationTransferConstructor
   {
      public static object CreateSimulationTransfer(string projectSnapshot)
      {
         var container = ApplicationStartup.Initialize();
         var projectSnapshotToSimulationTransferMapper = container.Resolve<IProjectSnapshotToSimulationTransferMapper>();
         var moBiExportTask = container.Resolve<IMoBiExportTask>();

         var simulationTransfer = projectSnapshotToSimulationTransferMapper.MapFrom(projectSnapshot);
         return moBiExportTask.Serialize(simulationTransfer);
      }
   }
}