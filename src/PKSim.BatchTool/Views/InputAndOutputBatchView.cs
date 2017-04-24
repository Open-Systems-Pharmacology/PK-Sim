using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.BatchTool.Views
{
   public partial class InputAndOutputBatchView : BaseView, IInputAndOutputBatchView
   {
      private IInputAndOutputBatchPresenter _presenter;
      private readonly ScreenBinder<InputAndOutputBatchDTO> _screenBinder;

      public InputAndOutputBatchView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<InputAndOutputBatchDTO>();
      }

      public void AttachPresenter(IInputAndOutputBatchPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(InputAndOutputBatchDTO batchDTO)
      {
         _screenBinder.BindToSource(batchDTO);
      }

      public bool CalculateEnabled
      {
         set { btnCalculate.Enabled = value; }
      }

      public void Display()
      {
         Show();
      }

      public void AddLogView(IView view)
      {
         panelLog.FillWith(view);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.InputFolder).To(btnInputFolder);
         _screenBinder.Bind(x => x.OutputFolder).To(btnOutputFolder);

         btnInputFolder.ButtonClick += (o, e) => OnEvent(_presenter.SelectInputFolder);
         btnOutputFolder.ButtonClick += (o, e) => OnEvent(_presenter.SelectOutputFolder);
         btnClose.Click += (o, e) => OnEvent(_presenter.Exit);
         btnCalculate.Click += (o, e) => OnEvent(async ()=> await _presenter.RunBatch());

         RegisterValidationFor(_screenBinder);
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
         btnCalculate.Enabled = !_screenBinder.HasError;
      }


      public override void InitializeResources()
      {
         base.InitializeResources();
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
      }
   }
}