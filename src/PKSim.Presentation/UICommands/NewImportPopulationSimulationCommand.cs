using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.UICommands
{
   public class NewImportPopulationSimulationCommand : IUICommand
   {
      private readonly ISimulationTask _simulationTask;
      private readonly IApplicationController _applicationController;

      public NewImportPopulationSimulationCommand(ISimulationTask simulationTask, IApplicationController applicationController)
      {
         _simulationTask = simulationTask;
         _applicationController = applicationController;
      }

      public void Execute()
      {
         Execute(string.Empty);
      }

      public virtual void Execute(string simulationFileName)
      {
         using (var presenter = _applicationController.Start<IImportPopulationSimulationPresenter>())
         {
            var simulation = presenter.CreateImportPopulationSimulation(simulationFileName);
            if (simulation == null) return;

            _simulationTask.AddToProject(simulation, editBuildingBlock: true);
         }
      }
   }
}