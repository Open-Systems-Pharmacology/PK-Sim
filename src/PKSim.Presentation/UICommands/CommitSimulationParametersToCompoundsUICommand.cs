using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation.UICommands
{
   public class CommitSimulationParametersToCompoundsUICommand : ObjectUICommand<Simulation>
   {
      private readonly IApplicationController _applicationController;
      private readonly ICommitSimulationParametersTask _commitTask;
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly ILazyLoadTask _lazyLoadTask;

      /// <summary>
      ///    Optional: when set, only show parameters for this compound in the dialog.
      /// </summary>
      public Compound CompoundFilter { get; set; }

      public CommitSimulationParametersToCompoundsUICommand(
         IApplicationController applicationController,
         ICommitSimulationParametersTask commitTask,
         IBuildingBlockTask buildingBlockTask,
         ILazyLoadTask lazyLoadTask)
      {
         _applicationController = applicationController;
         _commitTask = commitTask;
         _buildingBlockTask = buildingBlockTask;
         _lazyLoadTask = lazyLoadTask;
      }

      protected override void PerformExecute()
      {
         _lazyLoadTask.Load(Subject);

         using (var presenter = _applicationController.Start<ICommitSimulationParametersPresenter>())
         {
            var commitInfos = presenter.ShowCommitDialog(Subject, CompoundFilter);
            if (commitInfos == null || commitInfos.Count == 0)
               return;

            var macroCommand = _commitTask.CommitParametersToCompounds(Subject, commitInfos);
            _buildingBlockTask.AddCommandToHistory(macroCommand);
         }
      }
   }
}
