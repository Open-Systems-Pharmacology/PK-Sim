using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation;
using OSPSuite.UI.Core;

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
         DragDrop += (o,e)=> _individualSimulationComparisonPresenter.DragDrop(e, new DragEvent(e));
         DragOver += (o, e) => _individualSimulationComparisonPresenter.DragOver(e, new DragEvent(e));
      }

      public void AttachPresenter(IIndividualSimulationComparisonPresenter presenter)
      {
         _individualSimulationComparisonPresenter = presenter;
         _presenter = presenter;
      }

      public ApplicationIcon Image => ApplicationIcons.IndividualSimulationComparison;

   }
}