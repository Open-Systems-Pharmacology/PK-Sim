using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using static OSPSuite.UI.UIConstants.Size;
using Padding = System.Windows.Forms.Padding;

namespace PKSim.UI.Views.Simulations
{
   public partial class IndividualSimulationSettingsView : BaseModalView, IIndividualSimulationSettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IIndividualSimulationSettingsPresenter _presenter;
      private readonly UxDropDownButton _uxDropDownButton;

      public IndividualSimulationSettingsView(Shell shell, IToolTipCreator toolTipCreator)
         : base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.SIMULATION_SETTINGS_WIDTH, UIConstants.Size.SIMULATION_SETTINGS_HEIGHT);
         _uxDropDownButton = new UxDropDownButton();
      }

      public void AttachPresenter(IIndividualSimulationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = ButtonOk;
      }

      public void AddSettingsView(IView settingsView)
      {
         panel.FillWith(settingsView);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.IndividualSimulationSettings;
         Icon = ApplicationIcons.Simulation;
         _presenter.UpdateSaveSettingsButtonItem(_toolTipCreator, _uxDropDownButton);
         ReplaceExtraButtonWith(_uxDropDownButton);
         tablePanel.AdjustLongButtonWidth(_uxDropDownButton);
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }
   }
}