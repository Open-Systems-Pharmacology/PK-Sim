using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.Utility;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core.Services;

namespace PKSim.BatchTool.Views
{
   public partial class JsonSimulationBatchView : BatchView, IJsonSimulationBatchView
   {
      private IJsonSimulationBatchPresenter _presenter;
      private readonly ScreenBinder<JsonRunOptions> _screenBinder = new ScreenBinder<JsonRunOptions>();

      public JsonSimulationBatchView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IJsonSimulationBatchPresenter presenter)
      {
         _presenter = presenter;
         base.AttachPresenter(presenter);
      }

      public override bool CalculateEnabled
      {
         set => btnCalculate.Enabled = value;
      }

      public override void AddLogView(IView view)
      {
         panelLog.FillWith(view);
      }

      public void BindTo(JsonRunOptions startOptions)
      {
         _screenBinder.BindToSource(startOptions);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.InputFolder).To(btnInputFolder);
         _screenBinder.Bind(x => x.OutputFolder).To(btnOutputFolder);
         _screenBinder.Bind(x => x.JacobianUse)
            .To(cbJacobianUse)
            .WithValues(EnumHelper.AllValuesFor<JacobianUse>())
            .AndDisplays(x => x.ToString().SplitToUpperCase());

         btnInputFolder.ButtonClick += (o, e) => OnEvent(() => _presenter.SelectInputFolder());
         btnOutputFolder.ButtonClick += (o, e) => OnEvent(() => _presenter.SelectOutputFolder());
         btnClose.Click += (o, e) => OnEvent(() => _presenter.Exit());
         btnCalculate.Click += (o, e) => OnEvent(async () => await _presenter.RunBatch());
      }

      protected override void SetOkButtonEnable()
      {
         btnCalculate.Enabled = !_screenBinder.HasError;
      }
   }
}