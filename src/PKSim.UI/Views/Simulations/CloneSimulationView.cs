using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class CloneSimulationView : CreateSimulationView, ICloneSimulationView
   {
      public CloneSimulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.SimulationClone;
         Caption = PKSimConstants.UI.CloneSimulation;
      }
   }
}