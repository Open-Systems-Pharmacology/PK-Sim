using OSPSuite.Assets;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

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

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.PopulationSimulationComparison;
      }
   }
}