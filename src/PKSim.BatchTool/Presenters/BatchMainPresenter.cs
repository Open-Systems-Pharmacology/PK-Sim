using PKSim.BatchTool.Views;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchMainPresenter : IPresenter<IBatchMainView>
   {
      void Initialize(BatchStartOptions startOptions);
      void StartBatchRun();
      void StartBatchComparison();
      void GenerateTrainingMaterial();
      void GenerateProjectOverview();
   }

   public class BatchMainPresenter : AbstractPresenter<IBatchMainView, IBatchMainPresenter>, IBatchMainPresenter
   {
      private readonly IApplicationController _applicationController;
      private BatchStartOptions _startOptions;

      public BatchMainPresenter(IBatchMainView view, IApplicationController applicationController) : base(view)
      {
         _applicationController = applicationController;
         _startOptions = new BatchStartOptions();
      }

      public void Initialize(BatchStartOptions startOptions)
      {
         _startOptions = startOptions;
      }

      public void StartBatchRun()
      {
         start<IJsonSimulationBatchPresenter>();
      }

      public void StartBatchComparison()
      {
         start<IProjectComparisonPresenter>();
      }

      public void GenerateTrainingMaterial()
      {
         start<IGenerateTrainingMaterialPresenter>();
      }

      private void start<T>() where T : IInitializablePresenter<BatchStartOptions>
      {
         var presenter = _applicationController.Start<T>();
         _view.Hide();
         presenter.InitializeWith(_startOptions);
      }

      public void GenerateProjectOverview()
      {
         start<IGenerateProjectOverviewPresenter>();
      }
   }
}