using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisResultsPresenter : ContextSpecification<IPopulationAnalysisResultsPresenter>
   {
      protected IBoxWhiskerAnalysisResultsView _view;
      protected IBoxWhiskerFieldSelectionPresenter _fieldSelectionPresenter;
      protected IBoxWhiskerChartPresenter _chartPresenter;
      protected IBoxWhiskerChartDataCreator _chartDataCreator;
      protected IPopulationAnalysisTask _populationAnalysisTask;
      protected PopulationPivotAnalysis _populationAnalysis;
      protected IPopulationDataCollector _dataCollector;

      protected override void Context()
      {
         _view = A.Fake<IBoxWhiskerAnalysisResultsView>();
         _fieldSelectionPresenter = A.Fake<IBoxWhiskerFieldSelectionPresenter>();
         _chartPresenter = A.Fake<IBoxWhiskerChartPresenter>();
         _chartDataCreator = A.Fake<IBoxWhiskerChartDataCreator>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();
         sut = new BoxWhiskerAnalysisResultsPresenter(_view,_fieldSelectionPresenter,_chartPresenter,_chartDataCreator,_populationAnalysisTask);

         _dataCollector= A.Fake<IPopulationDataCollector>();
         _dataCollector= A.Fake<IPopulationDataCollector>();
         sut.Chart = new BoxWhiskerAnalysisChart().WithName("Chart");
      }
   }

   public class When_notify_that_the_user_wants_to_export_the_selected_results_in_create_mode : concern_for_PopulationAnalysisResultsPresenter
   {
      private  ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         sut.StartAnalysis(_dataCollector, _populationAnalysis);
         _chartData = ChartDataHelperForSpecs.CreateBoxWhiskerChartData();
         A.CallTo(_chartDataCreator).WithReturnType<ChartData<BoxWhiskerXValue, BoxWhiskerYValue>>().Returns(_chartData);
      }

      protected override void Because()
      {
         _chartPresenter.OnExportDataToExcel += Raise.With(new EventArgs());
      }

      [Observation]
      public void should_levrage_the_analysis_task_to_export_the_current_chart()
      {
         A.CallTo(() => _populationAnalysisTask.ExportToExcel(_chartData, "Chart")).MustHaveHappened();
      }
   }
}	