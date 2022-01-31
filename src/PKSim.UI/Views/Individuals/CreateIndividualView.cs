using System.Drawing;
using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Individuals
{
   public partial class CreateIndividualView : BuildingBlockWizardView, ICreateIndividualView
   {
      public CreateIndividualView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.INDIVIDUAL_VIEW_WIDTH, UIConstants.Size.INDIVIDUAL_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateIndividualPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Individual.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateIndividual;
      }
   }
}