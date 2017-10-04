using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.BatchTool.Presenters;
using PKSim.Core;

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
         btnSnapshotsRun.Text = "Start Snapshots Run";
         Caption = CoreConstants.PRODUCT_NAME;
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
         ApplicationIcon = ApplicationIcons.PKSim;
         Icon = ApplicationIcon.WithSize(IconSizes.Size32x32);
      }

      public override void InitializeBinding()
      {
         btnStartBatchRun.Click += (o, e) => OnEvent(() => _presenter.StartBatchRun());
         btnStartProjectComparison.Click += (o, e) => OnEvent(() => _presenter.StartProjectComparison());
         btnGenerateTrainingMaterial.Click += (o, e) => OnEvent(() => _presenter.GenerateTrainingMaterial());
         btnGenerateProjectOverview.Click += (o, e) => OnEvent(() => _presenter.GenerateProjectOverview());
         btnSnapshotsRun.Click += (o, e) => OnEvent(() => _presenter.StartSnapshotsRun());
      }
   }
}