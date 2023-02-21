using System.Drawing;
using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Individuals
{
   public partial class CreateIndividualView : BuildingBlockWizardView, ICreateIndividualView
   {

      //TODO: This should not be used. Commenting out for now, just in case we need it again because LookAndFell
      //keeps getting reset when the PKSimStarter starts
      
      // public CreateIndividualView(Shell shell) : this(shell as BaseShell)
      // {
      //    
      // }

      public CreateIndividualView(BaseShell shell) : base(shell)
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
         ApplicationIcon = ApplicationIcons.Individual;
         Caption = PKSimConstants.UI.CreateIndividual;
      }
   }
}