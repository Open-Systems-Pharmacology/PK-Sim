using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.BatchTool.Views
{
   public partial class InputAndOutputBatchView<TStartOptions> : BatchView, IInputAndOutputBatchView<TStartOptions> where TStartOptions : IWithInputAndOutputFolders
   {
      private IInputAndOutputBatchPresenter _presenter;
      private readonly ScreenBinder<TStartOptions> _screenBinder = new ScreenBinder<TStartOptions>();

      public InputAndOutputBatchView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IInputAndOutputBatchPresenter presenter)
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

      public void BindTo(TStartOptions startOptions)
      {
         _screenBinder.BindToSource(startOptions);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.InputFolder).To(btnInputFolder);
         _screenBinder.Bind(x => x.OutputFolder).To(btnOutputFolder);

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