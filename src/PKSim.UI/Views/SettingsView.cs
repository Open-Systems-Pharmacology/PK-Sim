using DevExpress.XtraTab;
using PKSim.Assets;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class SettingsView : BaseModalTabbedContainerView,ISettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private ISettingsPresenter _presenter;

      public SettingsView(Shell shell,IToolTipCreator toolTipCreator): base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
      }

      public void AttachPresenter(ISettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override XtraTabControl TabControl
      {
         get { return tabSettings; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ExtraVisible = true;
         Icon = ApplicationIcons.Settings;
         Caption = PKSimConstants.UI.Options;
         btnExtra.Text = PKSimConstants.UI.ResetLayout;
         btnExtra.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ResetLayoutSettingsToolTip);
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

      protected override int TopicId => HelpId.PKSim_Options;
   }
}