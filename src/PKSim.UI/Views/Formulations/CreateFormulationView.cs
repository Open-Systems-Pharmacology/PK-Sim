using System.Drawing;
using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Views.Formulations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Formulations
{
   public partial class CreateFormulationView : BuildingBlockContainerView, ICreateFormulationView
   {
      public CreateFormulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.FORMULATION_VIEW_WIDTH, UIConstants.Size.FORMULATION_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateFormulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Formulation;
         Caption = PKSimConstants.UI.CreateFormulation;
      }
   }
}