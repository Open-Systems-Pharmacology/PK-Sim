using System.Drawing;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class PopulationSimulationSettingsView : BaseModalView, IPopulationSimulationSettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IPopulationSimulationSettingsPresenter _presenter;
      private readonly UxDropDownButton _uxDropDownButton = new UxDropDownButton();

      public PopulationSimulationSettingsView(Shell shell, IToolTipCreator toolTipCreator)
         : base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.SIMULATION_SETTINGS_WIDTH, UIConstants.Size.SIMULATION_SETTINGS_HEIGHT);
      }

      public void AttachPresenter(IPopulationSimulationSettingsPresenter presenter)
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
         Caption = PKSimConstants.UI.PopulationSimulationSettings;
         _presenter.UpdateSaveSettingsButtonItem(_toolTipCreator, _uxDropDownButton);
         ReplaceExtraButtonWith(_uxDropDownButton);
         tablePanel.AdjustLongButtonWidth(_uxDropDownButton);
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }
   }
}