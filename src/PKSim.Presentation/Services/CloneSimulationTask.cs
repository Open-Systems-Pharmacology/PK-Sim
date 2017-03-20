using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class CloneSimulationTask : ICloneSimulationTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IExecutionContext _executionContext;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ISimulationResultsTask _simulationResultsTask;
      private readonly IApplicationController _applicationController;

      public CloneSimulationTask(IBuildingBlockTask buildingBlockTask, IExecutionContext executionContext, IBuildingBlockInSimulationManager buildingBlockInSimulationManager,
         ISimulationSettingsRetriever simulationSettingsRetriever, ISimulationResultsTask simulationResultsTask, IApplicationController applicationController)
      {
         _buildingBlockTask = buildingBlockTask;
         _executionContext = executionContext;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _simulationResultsTask = simulationResultsTask;
         _applicationController = applicationController;
      }

      public void Clone(Simulation simulationToClone)
      {
         if (_buildingBlockInSimulationManager.StatusFor(simulationToClone) != BuildingBlockStatus.Green)
            throw new PKSimException(PKSimConstants.Error.SimulationCloneOnlyAvailableWhenBuildingBlocksAreUptodate);

         _buildingBlockTask.Load(simulationToClone);

         using (var presenter = _applicationController.Start<ICloneSimulationPresenter>())
         {
            var cloneCommand = presenter.CloneSimulation(simulationToClone);
            //User cancel action. return
            if (cloneCommand.IsEmpty()) return;

            var clone = presenter.Simulation;

            _simulationResultsTask.CloneResults(simulationToClone, clone);
            clone.Creation.AsCloneOf(simulationToClone);
            _simulationSettingsRetriever.SynchronizeSettingsIn(clone);

            var addCommand = new AddBuildingBlockToProjectCommand(clone, _executionContext).Run(_executionContext);
            addCommand.Description = PKSimConstants.Command.CloneEntity.FormatWith(PKSimConstants.ObjectTypes.Simulation, simulationToClone.Name, clone.Name);
            _buildingBlockTask.AddCommandToHistory(addCommand);

            //after clone => we go in edit mode
            _buildingBlockTask.Edit(clone);
         }
      }
   }
}