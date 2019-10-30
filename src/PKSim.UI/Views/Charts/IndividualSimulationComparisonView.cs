using OSPSuite.Assets;
using OSPSuite.UI.Core;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;

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
         DragDrop += (o, e) => _individualSimulationComparisonPresenter.DragDrop(o, new DragEvent(e));
         DragOver += (o, e) => _individualSimulationComparisonPresenter.DragOver(o, new DragEvent(e));
      }

      public void AttachPresenter(IIndividualSimulationComparisonPresenter presenter)
      {
         _individualSimulationComparisonPresenter = presenter;
         _presenter = presenter;
      }

      public ApplicationIcon Image => ApplicationIcons.IndividualSimulationComparison;
   }
}