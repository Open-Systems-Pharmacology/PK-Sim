using System;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ITimeProfileChartSettingsPresenter : IPopulationAnalysisChartSettingsPresenter
   {
      void BindTo(ObservedDataCollection observedDataCollection);
      event Action ObservedDataSettingsChanged;
   }

   public class TimeProfileChartSettingsPresenter : AbstractPresenter<ITimeProfileChartSettingsView, ITimeProfileChartSettingsPresenter>, ITimeProfileChartSettingsPresenter
   {
      private readonly IPopulationAnalysisObservedDataSettingsPresenter _observedDataSettingsPresenter;
      private readonly IPopulationAnalysisChartSettingsPresenter _populationAnalysisChartSettingsPresenter;
      public event Action ObservedDataSettingsChanged = delegate { };

      public TimeProfileChartSettingsPresenter(ITimeProfileChartSettingsView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter,
         IPopulationAnalysisObservedDataSettingsPresenter observedDataSettingsPresenter) : base(view)
      {
         _populationAnalysisChartSettingsPresenter = populationAnalysisChartSettingsPresenter;
         var chartSettingsPresenter = populationAnalysisChartSettingsPresenter.ChartSettingsPresenter;
         var chartExportSettingsPresenter = populationAnalysisChartSettingsPresenter.ChartExportSettingsPresenter;
         _observedDataSettingsPresenter = observedDataSettingsPresenter;
         view.AddChartExportSettingsView(chartExportSettingsPresenter.BaseView);
         view.AddChartSettingsView(chartSettingsPresenter.BaseView);
         view.AddObservedDataSettingsView(_observedDataSettingsPresenter.BaseView);
         AddSubPresenters(populationAnalysisChartSettingsPresenter, observedDataSettingsPresenter);
         _subPresenterManager.InitializeWith(this);
         _observedDataSettingsPresenter.StatusChanged += (o, e) => ObservedDataSettingsChanged();
      }

      public void EditConfiguration()
      {
         _populationAnalysisChartSettingsPresenter.EditConfiguration();
      }

      public void SetEditConfigurationAction(Action editAction)
      {
         _populationAnalysisChartSettingsPresenter.SetEditConfigurationAction(editAction);
      }

      public void Edit(PopulationAnalysisChart populationAnalysisChart)
      {
         _populationAnalysisChartSettingsPresenter.Edit(populationAnalysisChart);
      }

      public bool AllowEdit
      {
         get { return _populationAnalysisChartSettingsPresenter.AllowEdit; }
         set { _populationAnalysisChartSettingsPresenter.AllowEdit = value; }
      }

      public IChartSettingsPresenter ChartSettingsPresenter
      {
         get { return _populationAnalysisChartSettingsPresenter.ChartSettingsPresenter; }
      }

      public IChartExportSettingsPresenter ChartExportSettingsPresenter
      {
         get { return _populationAnalysisChartSettingsPresenter.ChartExportSettingsPresenter; }
      }

      public void BindTo(ObservedDataCollection observedDataCollection)
      {
         _observedDataSettingsPresenter.Edit(observedDataCollection);
      }
   }
}