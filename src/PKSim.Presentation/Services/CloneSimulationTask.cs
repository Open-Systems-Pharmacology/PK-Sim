using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Services
{
   public class CloneSimulationTask : ICloneSimulationTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ISimulationResultsTask _simulationResultsTask;
      private readonly IApplicationController _applicationController;
      private readonly IRenameBuildingBlockTask _renameBuildingBlockTask;

      public CloneSimulationTask(IBuildingBlockTask buildingBlockTask, IBuildingBlockInProjectManager buildingBlockInProjectManager,
         ISimulationSettingsRetriever simulationSettingsRetriever, ISimulationResultsTask simulationResultsTask, IApplicationController applicationController, IRenameBuildingBlockTask renameBuildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _simulationResultsTask = simulationResultsTask;
         _applicationController = applicationController;
         _renameBuildingBlockTask = renameBuildingBlockTask;
      }

      public void Clone(Simulation simulationToClone)
      {
         if (_buildingBlockInProjectManager.StatusFor(simulationToClone) != BuildingBlockStatus.Green)
            throw new PKSimException(PKSimConstants.Error.SimulationCloneOnlyAvailableWhenBuildingBlocksAreUptodate);

         _buildingBlockTask.Load(simulationToClone);

         using (var presenter = _applicationController.Start<ICloneSimulationPresenter>())
         {
            var cloneCommand = presenter.CloneSimulation(simulationToClone);
            //User cancel action. return
            if (cloneCommand.IsEmpty()) return;

            var clone = presenter.Simulation;

            _simulationResultsTask.CloneResults(simulationToClone, clone);

            // The simulation must be renamed after results are added to the simulation
            _renameBuildingBlockTask.RenameSimulation(clone, presenter.CloneName);

            clone.Creation.AsCloneOf(simulationToClone);
            _simulationSettingsRetriever.SynchronizeSettingsIn(clone);

            var addCommand = _buildingBlockTask.AddToProject(clone, editBuildingBlock: true, addToHistory: false);
            addCommand.Description = PKSimConstants.Command.CloneEntity(PKSimConstants.ObjectTypes.Simulation, simulationToClone.Name, clone.Name);
            _buildingBlockTask.AddCommandToHistory(addCommand);
         }
      }
   }
}