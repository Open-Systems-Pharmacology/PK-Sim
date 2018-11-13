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
      }

      public void AttachPresenter(ICreateCompoundPresenter presenter)
      {
         WizardPresenter = presenter;
      }
     
      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Compound.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateCompound;
      }
   }
}