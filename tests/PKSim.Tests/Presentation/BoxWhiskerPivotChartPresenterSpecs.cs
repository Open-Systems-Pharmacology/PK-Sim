using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_BoxWhiskerPivotChartPresenter : ContextSpecification<IBoxWhiskerAnalysisResultsPresenter>
   {
      private IBoxWhiskerAnalysisResultsView _view;
      protected IBoxWhiskerFieldSelectionPresenter _fieldSelectionPresenter;
      private IBoxWhiskerChartPresenter _boxWhiskerChartPresenter;
      private IBoxWhiskerChartDataCreator _boxWhiskerChartDataCreator;
      private IPopulationAnalysisTask _populationAnalysisTask;

      protected override void Context()
      {
         _view = A.Fake<IBoxWhiskerAnalysisResultsView>();
         _fieldSelectionPresenter = A.Fake<IBoxWhiskerFieldSelectionPresenter>();
         _boxWhiskerChartPresenter = A.Fake<IBoxWhiskerChartPresenter>();
         _boxWhiskerChartDataCreator = A.Fake<IBoxWhiskerChartDataCreator>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();
         sut = new BoxWhiskerAnalysisResultsPresenter(_view, _fieldSelectionPresenter, _boxWhiskerChartPresenter, _boxWhiskerChartDataCreator, _populationAnalysisTask);
      }
   }
}