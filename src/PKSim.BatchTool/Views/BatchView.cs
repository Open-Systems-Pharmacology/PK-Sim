using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Views;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool.Views
{
   public partial class BatchView : BaseView, IView<IBatchPresenter>
   {
      private IBatchPresenter _presenter;
      public virtual bool CalculateEnabled { get; set; }

      public BatchView()
      {
         InitializeComponent();
      }

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
         {
            e.Cancel = !_presenter.Exit();
         }

         base.OnFormClosing(e);
      }

      protected override void OnValidationError(Control control, string error)
      {
         base.OnValidationError(control, error);
         SetOkButtonEnable();
      }

      protected override void OnClearError(Control control)
      {
         base.OnClearError(control);
         SetOkButtonEnable();
      }

      protected virtual void SetOkButtonEnable()
      {
      }

      public void AttachPresenter(IBatchPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
         Icon = ApplicationIcons.PKSim.WithSize(IconSizes.Size32x32);
      }

      public virtual void AddLogView(IView view)
      {
      }
   }
}