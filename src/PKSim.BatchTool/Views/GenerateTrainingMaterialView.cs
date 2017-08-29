using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.BatchTool.Views
{
   public partial class GenerateTrainingMaterialView : BaseView, IGenerateTrainingMaterialView
   {
      private IGenerateTrainingMaterialPresenter _presenter;
      private readonly ScreenBinder<TrainingMaterialsOptions> _screenBinder;

      public GenerateTrainingMaterialView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<TrainingMaterialsOptions>();
      }

      public void AttachPresenter(IGenerateTrainingMaterialPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
         layoutItemLog.TextVisible = false;
         btnCancel.InitWithImage(ApplicationIcons.Exit, "Exit");
         btnGenerate.InitWithImage(ApplicationIcons.Run, "Generate");
         layoutItemOutputFolder.Text = "Output Folder".FormatForLabel();
         layoutItemButtonCancel.AdjustLargeButtonSize();
         layoutItemButtonGenerate.AdjustLargeButtonSize();
         Caption = "Training Material Generator";
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.OutputFolder).To(btnOutputFolder);

         btnOutputFolder.ButtonClick += (o, e) => OnEvent(_presenter.SelectOutputFolder);
         btnCancel.Click += (o, e) => OnEvent(()=>_presenter.Exit());
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

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
         {
            e.Cancel = !_presenter.Exit();
         }

         base.OnFormClosing(e);
      }

      protected virtual void SetOkButtonEnable()
      {
         btnGenerate.Enabled = !_screenBinder.HasError;
      }

      public void BindTo(TrainingMaterialsOptions batchDTO)
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
   }
}