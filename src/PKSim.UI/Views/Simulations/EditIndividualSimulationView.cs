using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class EditIndividualSimulationView : EditAnalyzableView, IEditIndividualSimulationView
   {
      public EditIndividualSimulationView(Shell shell, IImageListRetriever imageListRetriever)
         : base(shell, imageListRetriever)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditIndividualSimulationPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}