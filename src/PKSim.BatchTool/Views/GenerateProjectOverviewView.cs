using System.Windows.Forms;
using PKSim.BatchTool.Presenters;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.BatchTool.Views
{
   public partial class GenerateProjectOverviewView : BaseView, IGenerateProjectOverviewView
   {
      private IGenerateProjectOverviewPresenter _presenter;
      private readonly ScreenBinder<OutputBatchDTO> _screenBinder;

      public GenerateProjectOverviewView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<OutputBatchDTO>();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
         btnGenerate.InitWithImage(ApplicationIcons.Run, "Generate");
         layoutItemOutputFolder.Text = "Output Folder".FormatForLabel();
         Caption = "Generate Project Overview";
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.OutputFolder).To(btnInputFolder);

         btnInputFolder.ButtonClick += (o, e) => OnEvent(_presenter.SelectInputFolder);
         btnGenerate.Click += (o, e) => OnEvent(async ()=> await _presenter.RunBatch());

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
         btnGenerate.Enabled = !_screenBinder.HasError;
      }

      public void BindTo(OutputBatchDTO batchDTO)
      {
         _screenBinder.BindToSource(batchDTO);
      }

      public bool CalculateEnabled
      {
         set { btnGenerate.Enabled = value; }
      }

      public void Display()
      {
         Show();
      }

      public void AddLogView(IView view)
      {
         panelLog.FillWith(view);
      }

      public void AttachPresenter(IGenerateProjectOverviewPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}