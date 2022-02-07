﻿using System.Drawing;
using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraTab;
using PKSim.Core;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class ConfigureSimulationView : WizardView, IConfigureSimulationView
   {
      public ConfigureSimulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.SIMULATION_VIEW_WIDTH, UIConstants.Size.CONFIGURE_SIMULATION_VIEW_HEIGHT);
      }

      public override XtraTabControl TabControl => tabConfigureSimulation;

      public void AttachPresenter(ISimulationWizardPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.SimulationConfigure.WithSize(IconSizes.Size16x16);
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
         btnOk.DialogResult = DialogResult.None;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnOk.Click += (o, e) => OnEvent(simulationWizardPresenter.CreateSimulation);
      }

      private ISimulationWizardPresenter simulationWizardPresenter => WizardPresenter.DowncastTo<ISimulationWizardPresenter>();
   }
}