using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadSimulationFromSnapshotUICommand : IUICommand
   {
      private readonly IApplicationController _applicationController;
      private readonly ISimulationTask _simulationTask;

      public LoadSimulationFromSnapshotUICommand(IApplicationController applicationController, ISimulationTask simulationTask)
      {
         _applicationController = applicationController;
         _simulationTask = simulationTask;
      }

      public void Execute()
      {
         using (var presenter = _applicationController.Start<ILoadSimulationFromSnapshotPresenter>())
         {
            var simulation = presenter.LoadSimulation();
            if (simulation != null)
               _simulationTask.AddToProject(simulation);
         }
      }
   }
}