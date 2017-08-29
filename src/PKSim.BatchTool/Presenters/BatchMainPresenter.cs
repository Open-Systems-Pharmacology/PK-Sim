using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Views;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchMainPresenter : IPresenter<IBatchMainView>
   {
      void StartBatchRun(JsonRunOptions jsonRunOptions = null);
      void StartProjectComparison(ProjectComparisonOptions projectComparisonOptions = null);
      void GenerateTrainingMaterial(TrainingMaterialsOptions trainingMaterialsOptions = null);
      void GenerateProjectOverview(ProjectOverviewOptions projectOverviewOptions = null);
   }

   public class BatchMainPresenter : AbstractPresenter<IBatchMainView, IBatchMainPresenter>, IBatchMainPresenter
   {
      private readonly IApplicationController _applicationController;

      public BatchMainPresenter(IBatchMainView view, IApplicationController applicationController) : base(view)
      {
         _applicationController = applicationController;
      }

      public void StartBatchRun(JsonRunOptions jsonRunOptions = null)
      {
         start<IJsonSimulationBatchPresenter, JsonRunOptions>(jsonRunOptions);
      }

      public void StartProjectComparison(ProjectComparisonOptions projectComparisonOptions = null)
      {
         start<IProjectComparisonPresenter, ProjectComparisonOptions>(projectComparisonOptions);
      }

      public void GenerateTrainingMaterial(TrainingMaterialsOptions trainingMaterialsOptions = null)
      {
         start<IGenerateTrainingMaterialPresenter, TrainingMaterialsOptions>(trainingMaterialsOptions);
      }

      public void GenerateProjectOverview(ProjectOverviewOptions projectOverviewOptions = null)
      {
         start<IGenerateProjectOverviewPresenter, ProjectOverviewOptions>(projectOverviewOptions);
      }

      private T start<T, TStartOptions>(TStartOptions startOptions) where T : IBatchPresenter<TStartOptions>
      {
         var presenter = _applicationController.Start<T>();
         View.Hide();
         if(startOptions!=null)
            presenter.InitializeForCommandLineRunWith(startOptions);
         else
            presenter.InitializeForStandAloneStart();

         return presenter;
      }
   }
}