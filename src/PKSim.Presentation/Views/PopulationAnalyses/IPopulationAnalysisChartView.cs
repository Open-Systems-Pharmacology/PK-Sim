using System.Windows.Forms;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisChartView<TX, TY, TPresenter> : IView<TPresenter>, IChartWithSettings
      where TX : IXValue
      where TY : IYValue
      where TPresenter : IPopulationAnalysisChartPresenter<TX, TY>
   {
      IChartsDataBinder<TX, TY> ChartsDataBinder { get; }
      void ClearAllSeries();
      void AddDynamicMenus(bool allowEdit = true);

      /// <summary>
      /// Speficy if Drag and drop operations are enabled on view. Default is <c>false</c>
      /// </summary>
      bool DragDropEnabled { get; set; }

      /// <summary>
      ///    Event is fired when some data are dragged over view
      /// </summary>
      event DragEventHandler DragOver;

      /// <summary>
      ///    Event is fired when some data are dropped onto view
      /// </summary>
      event DragEventHandler DragDrop;
   }

   public interface IChartWithSettings
   {
      void SetChartSettingsEditor(IView view);
      void UpdateWatermark(PopulationAnalysisChart populationAnalysisChart,  bool showWatermark);
   }

   public interface IBoxWhiskerChartView : IPopulationAnalysisChartView<BoxWhiskerXValue, BoxWhiskerYValue, IBoxWhiskerChartPresenter>
   {
   }

   public interface IScatterChartView : IPopulationAnalysisChartView<ScatterXValue, ScatterYValue, IScatterChartPresenter>
   {
   }

   public interface IRangeChartView : IPopulationAnalysisChartView<RangeXValue, RangeYValue, IRangeChartPresenter>
   {
   }

   public interface ITimeProfileChartView : IPopulationAnalysisChartView<TimeProfileXValue, TimeProfileYValue, ITimeProfileChartPresenter>
   {
   }
}