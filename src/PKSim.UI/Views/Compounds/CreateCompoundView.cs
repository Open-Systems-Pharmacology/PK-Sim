using System.Drawing;
using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Compounds
{
   public partial class CreateCompoundView : BuildingBlockWizardView, ICreateCompoundView
   {
      public CreateCompoundView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.COMPOUND_VIEW_WIDTH, UIConstants.Size.COMPOUND_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateCompoundPresenter presenter)
      {
         WizardPresenter = presenter;
      }
     
      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Compound;
         Caption = PKSimConstants.UI.CreateCompound;
      }
   }
}