using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisResultsPresenter : IPopulationAnalysisItemPresenter,
      IListener<FieldsMovedInPopulationAnalysisEvent>,
      IListener<PopulationAnalysisDataSelectionChangedEvent>
   {
      /// <summary>
      ///    Refreshes the analysis. This should be called whenever the underlying data have changed or when fields were added
      ///    ore removed from the analysis.
      /// </summary>
      void RefreshAnalysis();

      /// <summary>
      ///    Refreshes the Chart. The underlying analysis will not be performed again
      /// </summary>
      void RefreshChart();

      PopulationAnalysisChart Chart { get; set; }
   }

   public abstract class PopulationAnalysisResultsPresenter<TXValue, TYValue, TView, TPresenter, TChartPresenter> : AbstractSubPresenter<TView, TPresenter>,
      IPopulationAnalysisResultsPresenter
      where TYValue : IYValue
      where TXValue : IXValue
      where TChartPresenter : IPopulationAnalysisChartPresenter<TXValue, TYValue>
      where TView : IView<TPresenter>, IPopulationAnalysisResultsView
      where TPresenter : IPresenter
   {
      protected readonly IPopulationAnalysisFieldSelectionPresenter _fieldSelectionPresenter;
      private readonly TChartPresenter _populationAnalysisChartPresenter;
      protected readonly IChartDataCreator<TXValue, TYValue> _chartDataCreator;
      private readonly IPopulationAnalysisTask _populationAnalysisTask;
      protected PopulationPivotAnalysis _populationPivotAnalysis;
      protected IPopulationDataCollector _populationDataCollector;
      public PopulationAnalysisChart Chart { get; set; }

      protected PopulationAnalysisResultsPresenter(TView view, IPopulationAnalysisFieldSelectionPresenter fieldSelectionPresenter,
         TChartPresenter populationAnalysisChartPresenter, IChartDataCreator<TXValue, TYValue> chartDataCreator, IPopulationAnalysisTask populationAnalysisTask)
         : base(view)
      {
         _fieldSelectionPresenter = fieldSelectionPresenter;
         _populationAnalysisChartPresenter = populationAnalysisChartPresenter;
         _chartDataCreator = chartDataCreator;
         _populationAnalysisTask = populationAnalysisTask;
         _chartDataCreator = chartDataCreator;
         _view.SetFieldSelectionView(_fieldSelectionPresenter.BaseView);
         _view.SetChartView(populationAnalysisChartPresenter.BaseView);
         _populationAnalysisChartPresenter.OnExportDataToExcel += (o, e) => exportDataToExcel();
         _populationAnalysisChartPresenter.OnExportToPDF += (o, e) => exportToPDF();

         populationAnalysisChartPresenter.Initialize();
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationPivotAnalysis = populationAnalysis.DowncastTo<PopulationPivotAnalysis>();
         _populationDataCollector = populationDataCollector;
         _fieldSelectionPresenter.StartAnalysis(populationDataCollector, _populationPivotAnalysis);
      }

      private bool canHandle(PopulationAnalysisEvent eventToHandle)
      {
         return Equals(eventToHandle.PopulationAnalysis, _populationPivotAnalysis);
      }

      public void Handle(PopulationAnalysisDataSelectionChangedEvent eventToHandle)
      {
         handle(eventToHandle);
      }

      private void handle(PopulationAnalysisEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         RefreshChart();
      }

      public void Handle(FieldsMovedInPopulationAnalysisEvent eventToHandle)
      {
         handle(eventToHandle);
      }

      protected abstract ChartData<TXValue, TYValue> CreateChartData();

      private void exportToPDF()
      {
         _populationAnalysisTask.ExportToPDF(Chart);
      }

      private void exportDataToExcel()
      {
         _populationAnalysisTask.ExportToExcel(CreateChartData(), Chart.Name);
      }

      public void RefreshChart()
      {
         var chartData = CreateChartData();
         _populationAnalysisChartPresenter.Show(chartData, Chart);
      }

      public void RefreshAnalysis()
      {
         _fieldSelectionPresenter.RefreshAnalysis();
         RefreshChart();
      }
   }
}