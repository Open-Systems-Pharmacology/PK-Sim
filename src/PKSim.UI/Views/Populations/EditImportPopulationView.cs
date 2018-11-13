using OSPSuite.Assets;
using DevExpress.XtraTab;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

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

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Population;

      public override XtraTabControl TabControl => tabEditPopulation;

   }
}