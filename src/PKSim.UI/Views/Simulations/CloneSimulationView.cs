using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation;

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
         Icon = ApplicationIcons.SimulationClone.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CloneSimulation;
      }
   }
}