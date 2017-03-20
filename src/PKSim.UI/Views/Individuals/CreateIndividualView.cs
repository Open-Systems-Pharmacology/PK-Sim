using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Individuals
{
   public partial class CreateIndividualView : BuildingBlockWizardView, ICreateIndividualView
   {
      public CreateIndividualView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new System.Drawing.Size(CoreConstants.UI.INDIVIDUAL_VIEW_WITDH, CoreConstants.UI.INDIVIDUAL_VIEW_HEIGHT);
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

      protected override int TopicId => HelpId.PKSim_Individuals_NewIndividual;
   }
}