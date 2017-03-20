using OSPSuite.UI.Services;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class PopulationSimulationComparisonView : EditAnalyzableView, IPopulationSimulationComparisonView
   {
      public PopulationSimulationComparisonView(Shell shell, IImageListRetriever imageListRetriever)
         : base(shell, imageListRetriever)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPopulationSimulationComparisonPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.PopulationSimulationComparison; }
      }
   }
}