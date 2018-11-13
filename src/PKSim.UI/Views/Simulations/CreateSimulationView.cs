using System.Drawing;
using System.Windows.Forms;
using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Simulations
{
   public partial class CreateSimulationView : BuildingBlockWizardView, ICreateSimulationView
   {
      public CreateSimulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(CoreConstants.UI.SIMULATION_VIEW_WITDH, CoreConstants.UI.CREATE_SIMULATION_VIEW_HEIGHT);
      }

      public void AttachPresenter(ISimulationWizardPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Simulation.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateSimulation;
         btnOk.DialogResult = DialogResult.None;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnOk.Click += (o, e) => OnEvent(simulationWizardPresenter.CreateSimulation);
      }
      private ISimulationWizardPresenter simulationWizardPresenter
      {
         get { return WizardPresenter.DowncastTo<ISimulationWizardPresenter>(); }
      }
   }
}