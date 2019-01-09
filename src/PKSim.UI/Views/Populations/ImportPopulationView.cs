using PKSim.Assets;
using OSPSuite.Assets;

using PKSim.Presentation.Presenters.Populations;

using PKSim.Presentation.Views.Populations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;

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
         Icon = ApplicationIcons.MergePopulation.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.ImportPopulation;
      }

   }
}