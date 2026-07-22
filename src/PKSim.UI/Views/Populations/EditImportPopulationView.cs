using DevExpress.XtraTab;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.UI.Views.Populations
{
   public partial class EditImportPopulationView : BaseMdiChildTabbedView, IEditImportPopulationView
   {
      public EditImportPopulationView(Shell shell): base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditImportPopulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Population;
      }

      public override XtraTabControl TabControl => tabEditPopulation;

   }
}