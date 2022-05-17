using DevExpress.XtraTab;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

namespace PKSim.UI.Views
{
   public partial class SettingsView : BaseModalTabbedContainerView, ISettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private ISettingsPresenter _presenter;

      public SettingsView(Shell shell, IToolTipCreator toolTipCreator) : base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
      }

      public void AttachPresenter(ISettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override XtraTabControl TabControl => tabSettings;

      public override void InitializeResources()
      {
         base.InitializeResources();
         ExtraVisible = true;
         ApplicationIcon = ApplicationIcons.Settings;
         Caption = PKSimConstants.UI.Options;
         ExtraCaption = PKSimConstants.UI.ResetLayout;
         ButtonExtra.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ResetLayoutSettingsToolTip);
         ActiveControl = TabControl;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = TabControl;
      }

      protected override void ExtraClicked()
      {
         _presenter.ResetLayout();
      }
   }
}