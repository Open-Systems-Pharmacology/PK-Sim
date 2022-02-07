using System.Drawing;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class IndividualSimulationSettingsView : BaseModalView, IIndividualSimulationSettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IIndividualSimulationSettingsPresenter _presenter;

      public IndividualSimulationSettingsView(Shell shell, IToolTipCreator toolTipCreator)
         : base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.SIMULATION_SETTINGS_WIDTH, UIConstants.Size.SIMULATION_SETTINGS_HEIGHT);
      }

      public void AttachPresenter(IIndividualSimulationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = btnOk;
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

         var dropDownButtonItem = _presenter.CreateSaveSettingsButtonItem(_toolTipCreator, layoutControlBase);
         dropDownButtonItem.Move(emptySpaceItemBase, InsertType.Left);
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }
   }
}