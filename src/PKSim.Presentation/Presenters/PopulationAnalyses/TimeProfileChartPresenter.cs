using System;
using System.Windows.Forms;
using OSPSuite.Presentation.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using PKSim.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ITimeProfileChartPresenter : IPresenter<ITimeProfileChartView>, IPopulationAnalysisChartPresenter<TimeProfileXValue, TimeProfileYValue>
   {
      /// <summary>
      ///    Event is fired when some data are dragged over view
      /// </summary>
      event EventHandler<IDragEvent> DragOver;

      /// <summary>
      ///    Event is fired when some data are dropped onto view
      /// </summary>
      event EventHandler<IDragEvent> DragDrop;

      event Action ObservedDataSettingsChanged;
   }

   internal class TimeProfileChartPresenter : PopulationAnalysisChartPresenter<ITimeProfileChartView, ITimeProfileChartPresenter, TimeProfileXValue, TimeProfileYValue>, ITimeProfileChartPresenter
   {
      private readonly ITimeProfileChartSettingsPresenter _timeProfileChartSettingsPresenter;
      public event Action ObservedDataSettingsChanged = delegate { };

      public TimeProfileChartPresenter(ITimeProfileChartView view, ITimeProfileChartSettingsPresenter timeProfileChartSettingsPresenter, IApplicationSettings applicationSettings)
         : base(view, timeProfileChartSettingsPresenter, applicationSettings)
      {
         view.DragDropEnabled = true;
         _timeProfileChartSettingsPresenter = timeProfileChartSettingsPresenter;
         _timeProfileChartSettingsPresenter.ObservedDataSettingsChanged += () => ObservedDataSettingsChanged();
      }

      public event EventHandler<IDragEvent> DragOver
      {
         add => _view.OnDragOverEvent += value;
         remove => _view.OnDragOverEvent -= value;
      }

      public event EventHandler<IDragEvent> DragDrop
      {
         add => _view.OnDragDropEvent += value;
         remove => _view.OnDragDropEvent -= value;
      }

      public override void Show(ChartData<TimeProfileXValue, TimeProfileYValue> chartsData, PopulationAnalysisChart populationAnalysisChart)
      {
         base.Show(chartsData, populationAnalysisChart);
         _timeProfileChartSettingsPresenter.BindTo(populationAnalysisChart.ObservedDataCollection);
      }
   }
}