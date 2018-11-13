using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Charts
{
   public partial class IndividualSimulationComparisonView : BasePKAnalysisWithChartView, IIndividualSimulationComparisonView
   {
      private IIndividualSimulationComparisonPresenter _individualSimulationComparisonPresenter;

      public IndividualSimulationComparisonView()
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         DragDrop += _individualSimulationComparisonPresenter.DragDrop;
         DragOver += _individualSimulationComparisonPresenter.DragOver;
      }

      public void AttachPresenter(IIndividualSimulationComparisonPresenter presenter)
      {
         _individualSimulationComparisonPresenter = presenter;
         _presenter = presenter;
      }

      public ApplicationIcon Image => ApplicationIcons.IndividualSimulationComparison;

   }
}