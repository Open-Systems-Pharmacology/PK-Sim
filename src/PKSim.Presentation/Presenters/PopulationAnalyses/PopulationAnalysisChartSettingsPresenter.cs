using System;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisChartSettingsPresenter : IPresenter
   {
      void EditConfiguration();
      void SetEditConfigurationAction(Action editAction);
      void Edit(PopulationAnalysisChart populationAnalysisChart);
      bool AllowEdit { get; set; }
      IChartSettingsPresenter ChartSettingsPresenter { get; }
      IChartExportSettingsPresenter ChartExportSettingsPresenter { get; }
   }

   public class PopulationAnalysisChartSettingsPresenter : AbstractPresenter<IPopulationAnalysisChartSettingsView, IPopulationAnalysisChartSettingsPresenter>, IPopulationAnalysisChartSettingsPresenter
   {
      private Action _editAction = () => { };
      private readonly IChartSettingsPresenter _chartSettingsPresenter;
      private readonly IChartExportSettingsPresenter _chartExportSettingsPresenter;

      public PopulationAnalysisChartSettingsPresenter(IPopulationAnalysisChartSettingsView view, IChartSettingsPresenter chartSettingsPresenter, IChartExportSettingsPresenter chartExportSettingsPresenter)
         : base(view)
      {
         _chartSettingsPresenter = chartSettingsPresenter;
         _chartExportSettingsPresenter = chartExportSettingsPresenter;
         view.SetChartSettingsView(chartSettingsPresenter.View);
         view.SetChartExportSettingsView(chartExportSettingsPresenter.View);
         view.AllowEditConfiguration = false;
         chartSettingsPresenter.NameVisible = false;
         AddSubPresenters(chartSettingsPresenter, chartExportSettingsPresenter);
         _subPresenterManager.InitializeWith(this);
      }

      public IChartSettingsPresenter ChartSettingsPresenter => _chartSettingsPresenter;

      public IChartExportSettingsPresenter ChartExportSettingsPresenter => _chartExportSettingsPresenter;

      public void EditConfiguration()
      {
         _editAction.Invoke();
      }

      public void SetEditConfigurationAction(Action editAction)
      {
         _editAction = editAction;
      }

      public void Edit(PopulationAnalysisChart populationAnalysisChart)
      {
         _chartExportSettingsPresenter.Edit(populationAnalysisChart);
         _chartSettingsPresenter.Edit(populationAnalysisChart);
      }

      public bool AllowEdit
      {
         get => _view.AllowEditConfiguration;
         set => _view.AllowEditConfiguration = value;
      }
   }
}