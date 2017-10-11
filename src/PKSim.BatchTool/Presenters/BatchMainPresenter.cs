using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Views;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchMainPresenter : IPresenter<IBatchMainView>
   {
      void StartBatchRun();
      void StartSnapshotsRun();
   }

   public class BatchMainPresenter : AbstractPresenter<IBatchMainView, IBatchMainPresenter>, IBatchMainPresenter
   {
      private readonly IApplicationController _applicationController;

      public BatchMainPresenter(IBatchMainView view, IApplicationController applicationController) : base(view)
      {
         _applicationController = applicationController;
      }

      public void StartBatchRun()
      {
         start<IJsonSimulationBatchPresenter>();
      }

      public void StartSnapshotsRun()
      {
         start<ISnapshotRunPresenter>();
      }

      private T start<T>() where T : IBatchPresenter
      {
         var presenter = _applicationController.Start<T>();
         View.Hide();
         presenter.InitializeForStandAloneStart();
         return presenter;
      }
   }
}