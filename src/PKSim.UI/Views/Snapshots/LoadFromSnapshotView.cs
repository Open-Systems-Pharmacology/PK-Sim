using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.UI.Views.Snapshots
{
   public partial class LoadFromSnapshotView : BaseModalView, ILoadFromSnapshotView
   {
      private ILoadFromSnapshotPresenter _presenter;
      private readonly ScreenBinder<LoadFromSnapshotDTO> _screenBinder = new ScreenBinder<LoadFromSnapshotDTO>();

      public LoadFromSnapshotView()
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.SnapshotFile)
            .To(buttonEditSelectSnapshot);

         buttonEditSelectSnapshot.ButtonClick += (o, e) => OnEvent(_presenter.SelectFile);
         buttonStart.Click += (o, e) => OnEvent(() => _presenter.Start());

         RegisterValidationFor(_screenBinder);
      }

      public void AttachPresenter(ILoadFromSnapshotPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddLogView(IView view)
      {
         logPanel.FillWith(view);
      }

      public void BindTo(LoadFromSnapshotDTO loadFromSnapshotDTO)
      {
         _screenBinder.BindToSource(loadFromSnapshotDTO);
      }

      public void EnableButtons(bool cancelEnabled, bool okEnabled = false, bool startEnabled = false)
      {
         btnCancel.Enabled = cancelEnabled;
         btnOk.Enabled = okEnabled;
         buttonStart.Enabled = startEnabled;
      }

      protected override void SetOkButtonEnable()
      {
         base.SetOkButtonEnable();
         buttonStart.Enabled = !HasError;
      }

      protected override bool IsOkButtonEnable => _presenter.ModelIsDefined;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemStartButton.AdjustLongButtonSize();
         buttonStart.InitWithImage(ApplicationIcons.Run, PKSimConstants.UI.StartImport);
         layoutItemButtonSelectSnapshot.Text = PKSimConstants.UI.SnapshotFile.FormatForLabel();
      }

      public override bool HasError => _screenBinder.HasError;
   }
}