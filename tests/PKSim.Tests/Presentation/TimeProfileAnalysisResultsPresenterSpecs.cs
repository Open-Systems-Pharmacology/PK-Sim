using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_TimeProfileAnalysisResultsPresenter : ContextSpecification<ITimeProfileAnalysisResultsPresenter>
   {
      private ITimeProfileAnalysisResultsView _view;
      protected ITimeProfileFieldSelectionPresenter _fieldSelectionPresenter;
      private ITimeProfileChartPresenter _timeProfileChartPresenter;
      private ITimeProfileChartDataCreator _timeProfileChartDataCreator;
      private IPopulationAnalysisTask _populationAnalysisTask;

      protected override void Context()
      {
         _view = A.Fake<ITimeProfileAnalysisResultsView>();
         _fieldSelectionPresenter = A.Fake<ITimeProfileFieldSelectionPresenter>();
         _timeProfileChartPresenter = A.Fake<ITimeProfileChartPresenter>();
         _timeProfileChartDataCreator = A.Fake<ITimeProfileChartDataCreator>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();

         sut = new TimeProfileAnalysisResultsPresenter(_view, _fieldSelectionPresenter, _timeProfileChartPresenter, _timeProfileChartDataCreator, _populationAnalysisTask);
      }
   }
}