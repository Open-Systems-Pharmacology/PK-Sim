using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Populations
{
   public partial class ImportPopulationView : BuildingBlockWizardView, IImportPopulationView
   {
      public ImportPopulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IImportPopulationPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.MergePopulation;
         Caption = PKSimConstants.UI.ImportPopulation;
      }

   }
}