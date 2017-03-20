using OSPSuite.UI.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class EditPopulationSimulationView : EditAnalyzableView, IEditPopulationSimulationView
   {
      public EditPopulationSimulationView(Shell shell, IImageListRetriever imageListRetriever)
         : base(shell, imageListRetriever)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditPopulationSimulationPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}