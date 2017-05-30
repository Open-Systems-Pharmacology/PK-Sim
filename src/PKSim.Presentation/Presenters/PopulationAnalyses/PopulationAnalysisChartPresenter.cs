﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisChartPresenter : IPresenter
   {
      /// <summary>
      ///    Specifies wether the edit method can be called
      /// </summary>
      bool AllowEdit { set; }

      PopulationAnalysisChart AnalysisChart { get; }

      /// <summary>
      ///    Starts the edit action
      /// </summary>
      void Edit();

      event EventHandler OnEdit;

      /// <summary>
      ///    Exports the underlying data to excel
      /// </summary>
      void ExportDataToExcel();

      event EventHandler OnExportDataToExcel;

      /// <summary>
      ///    Exports the plot to pdf
      /// </summary>
      void ExportToPDF();

      event EventHandler OnExportToPDF;

      void ClearPlot();
   }

   public interface IPopulationAnalysisChartPresenter<TX, TY> : IPopulationAnalysisChartPresenter
      where TX : IXValue
      where TY : IYValue
   {
      /// <summary>
      ///    Display the charts <paramref name="chartsData" /> using the specified <paramref name="populationAnalysisChart" />.
      /// </summary>
      void Show(ChartData<TX, TY> chartsData, PopulationAnalysisChart populationAnalysisChart);

      /// <summary>
      ///    Returns the curve data defined in the pane with id <paramref name="paneId" /> for the series with id
      ///    <paramref name="seriesId" />
      /// </summary>
      CurveData<TX, TY> CurveDataFor(string paneId, string seriesId);

      /// <summary>
      ///    Returns the observed curve data defined in the pane with id <paramref name="paneId" /> for the series with caption
      ///    <paramref name="caption" />
      /// </summary>
      ObservedCurveData ObservedCurveDataFor(string paneId, string caption);
   }

   public abstract class PopulationAnalysisChartPresenter<TView, TPresenter, TX, TY> : AbstractPresenter<TView, TPresenter>,
      IPopulationAnalysisChartPresenter<TX, TY>
      where TX : IXValue
      where TY : IYValue
      where TView : IPopulationAnalysisChartView<TX, TY, TPresenter>
      where TPresenter : IPopulationAnalysisChartPresenter<TX, TY>
   {
      private readonly IPopulationAnalysisChartSettingsPresenter _populationAnalysisChartSettingsPresenter;
      public event EventHandler OnEdit = delegate { };
      public event EventHandler OnExportDataToExcel = delegate { };
      public event EventHandler OnExportToPDF = delegate { };
      private readonly IChartsDataBinder<TX, TY> _chartDataBinder;
      private ChartData<TX, TY> _chartData;

      public PopulationAnalysisChart AnalysisChart { get; private set; }

      public virtual bool AllowEdit { get; set; }

      protected PopulationAnalysisChartPresenter(TView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter)
         : base(view)
      {
         _chartDataBinder = view.ChartsDataBinder;
         _populationAnalysisChartSettingsPresenter = populationAnalysisChartSettingsPresenter;
         _populationAnalysisChartSettingsPresenter.SetEditConfigurationAction(Edit);
         AddSubPresenters(_populationAnalysisChartSettingsPresenter);
         _view.SetChartSettingsEditor(_populationAnalysisChartSettingsPresenter.BaseView);
         _populationAnalysisChartSettingsPresenter.StatusChanged += SettingsChanged;
      }

      protected virtual void SettingsChanged(object o, EventArgs e)
      {
         _chartDataBinder.UpdateSettings(AnalysisChart);
      }

      public virtual void ClearPlot()
      {
         _chartDataBinder.ClearPlot();
      }

      public virtual void Show(ChartData<TX, TY> chartData, PopulationAnalysisChart populationAnalysisChart)
      {
         _chartData = chartData;
         AnalysisChart = populationAnalysisChart;
         _chartDataBinder.Bind(_chartData, populationAnalysisChart);
         _populationAnalysisChartSettingsPresenter.Edit(populationAnalysisChart);
      }

      public override void Initialize()
      {
         base.Initialize();
         View.AddDynamicMenus(AllowEdit);
         _populationAnalysisChartSettingsPresenter.AllowEdit = AllowEdit;
      }

      public virtual void Edit()
      {
         OnEdit(this, EventArgs.Empty);
      }

      public ObservedCurveData ObservedCurveDataFor(string paneId, string caption)
      {
         var paneData = _chartData.Panes[paneId];

         //Observed data can only be identifed using caption for now as their Id (observedData.Id) is not available in the DevExpress Series.
         return paneData?.ObservedCurveData.FirstOrDefault(o => string.Equals(o.Caption, caption));
      }

      public CurveData<TX, TY> CurveDataFor(string paneId, string seriesId)
      {
         var paneData = _chartData.Panes[paneId];
         if (paneData == null)
            return null;

         //either returns the serie with the given id or the first one if one is avalailable. 
         //series Id is empty for instance if we only deal with one serie in BoxWhisker 
         return paneData.Curves[seriesId] ?? paneData.Curves.FirstOrDefault();
      }

      public void ExportDataToExcel()
      {
         OnExportDataToExcel(this, EventArgs.Empty);
      }

      public void ExportToPDF()
      {
         OnExportToPDF(this, EventArgs.Empty);
      }
   }
}