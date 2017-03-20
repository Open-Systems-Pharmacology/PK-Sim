using OSPSuite.UI.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Views;

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