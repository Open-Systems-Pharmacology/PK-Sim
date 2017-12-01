using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.UI.Extensions;
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
         btnSnapshotsRun.Click += (o, e) => OnEvent(() => _presenter.StartSnapshotsRun());
      }
   }
}