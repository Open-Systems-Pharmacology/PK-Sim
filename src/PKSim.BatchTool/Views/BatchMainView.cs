using System.Windows.Forms;
using PKSim.BatchTool.Presenters;
using PKSim.Core;
using OSPSuite.UI.Views;

namespace PKSim.BatchTool.Views
{
   public partial class BatchMainView : BaseView, IBatchMainView
   {
      private IBatchMainPresenter _presenter;

      public BatchMainView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IBatchMainPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnStartBatchRun.Text = "Batch Run";
         btnStartProjectComparison.Text = "Project Comparison";
         btnGenerateTrainingMaterial.Text = "Generate Training Material";
         btnGenerateProjectOverview.Text = "Generate Project Compound and Observed Data";
         Caption = CoreConstants.ProductName;
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
      }

      public override void InitializeBinding()
      {
         btnStartBatchRun.Click += (o, e) => OnEvent(_presenter.StartBatchRun);
         btnStartProjectComparison.Click += (o, e) => OnEvent(_presenter.StartBatchComparison);
         btnGenerateTrainingMaterial.Click += (o, e) => OnEvent(_presenter.GenerateTrainingMaterial);
         btnGenerateProjectOverview.Click += (o, e) => OnEvent(_presenter.GenerateProjectOverview);
      }
   }
}