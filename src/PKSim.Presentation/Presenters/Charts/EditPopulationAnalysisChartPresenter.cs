using OSPSuite.Assets;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IEditPopulationAnalysisChartPresenter : ISimulationAnalysisPresenter<IPopulationDataCollector>,
      IListener<RenamedEvent>,
      IListener<SimulationResultsUpdatedEvent>
   {
   }

   public abstract class EditPopulationAnalysisChartPresenter<TPopulationAnalysisChart, TX, TY> : AbstractPresenter<ISimulationAnalysisChartView, IEditPopulationAnalysisChartPresenter>, IEditPopulationAnalysisChartPresenter
      where TPopulationAnalysisChart : PopulationAnalysisChart
      where TX : IXValue
      where TY : IYValue
   {
      protected readonly IPopulationAnalysisChartPresenter<TX, TY> _populationAnalysisChartPresenter;
      protected readonly IChartDataCreator<TX, TY> _chartDataCreator;
      private readonly IPopulationSimulationAnalysisStarter _populationSimulationAnalysisStarter;
      protected readonly IPopulationAnalysisTask _populationAnalysisTask;
      protected DefaultPresentationSettings _settings;
      protected TPopulationAnalysisChart PopulationAnalysisChart { get; private set; }

      protected EditPopulationAnalysisChartPresenter(ISimulationAnalysisChartView view, IPopulationAnalysisChartPresenter<TX, TY> populationAnalysisChartPresenter,
         IChartDataCreator<TX, TY> chartDataCreator, IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter, IPopulationAnalysisTask populationAnalysisTask, ApplicationIcon icon)
         : base(view)
      {
         _populationAnalysisChartPresenter = populationAnalysisChartPresenter;
         _chartDataCreator = chartDataCreator;
         _populationAnalysisChartPresenter.AllowEdit = true;
         _populationAnalysisChartPresenter.OnEdit += (o, e) => editPopulationAnalysis();
         _populationAnalysisChartPresenter.OnExportDataToExcel += (o, e) => exportDataToExcel();
         _populationAnalysisChartPresenter.OnExportToPDF += (o, e) => exportToPDF();
         _populationSimulationAnalysisStarter = populationSimulationAnalysisStarter;
         _populationAnalysisTask = populationAnalysisTask;
         View.SetChartView(_populationAnalysisChartPresenter.BaseView);
         View.UpdateIcon(icon);
         _populationAnalysisChartPresenter.Initialize();
         _settings = new DefaultPresentationSettings();
      }

      private void exportToPDF()
      {
         _populationAnalysisTask.ExportToPDF(Analysis);
      }

      private void exportDataToExcel()
      {
         _populationAnalysisTask.ExportToExcel(CreateChartData(), PopulationAnalysisChart.Name);
      }

      private void editPopulationAnalysis()
      {
         PopulationAnalysisChart = _populationSimulationAnalysisStarter.EditAnalysisForPopulationSimulation(PopulationDataCollector, PopulationAnalysisChart).DowncastTo<TPopulationAnalysisChart>();
         UpdateAnalysisBasedOn(PopulationDataCollector);
      }

      public void UpdateAnalysisBasedOn(IAnalysable analysable)
      {
         UpdateAnalysisBasedOn(analysable.DowncastTo<IPopulationDataCollector>());
      }

      public void UpdateAnalysisBasedOn(IPopulationDataCollector populationDataCollector)
      {
         PopulationAnalysisChart.Analysable = populationDataCollector;
         RefreshData();
      }

      protected IPopulationDataCollector PopulationDataCollector => PopulationAnalysisChart.Analysable.DowncastTo<IPopulationDataCollector>();

      protected virtual void RefreshData()
      {
         var chartData = CreateChartData();
         _populationAnalysisChartPresenter.Show(chartData, PopulationAnalysisChart);
      }

      protected abstract ChartData<TX, TY> CreateChartData();

      public void InitializeAnalysis(ISimulationAnalysis simulationAnalysis, IAnalysable analysable)
      {
         PopulationAnalysisChart = simulationAnalysis.DowncastTo<TPopulationAnalysisChart>();
         updateCaption();
         UpdateAnalysisBasedOn(analysable);
      }

      private void updateCaption()
      {
         View.Caption = PopulationAnalysisChart.Name;
      }

      private void updateOriginText()
      {
         _populationAnalysisTask.SetOriginText(PopulationAnalysisChart, PopulationDataCollector.Name);
      }

      public ISimulationAnalysisView AnalysisView => View;

      public ISimulationAnalysis Analysis => PopulationAnalysisChart;

      public virtual void Clear()
      {
         //necessary to dispose view here that was added dynamically to the container view and might not be disposed
         View.Dispose();
      }

      public void Handle(RenamedEvent eventToHandle)
      {
         if (!Equals(eventToHandle.RenamedObject, PopulationAnalysisChart))
            return;

         updateCaption();
      }

      public virtual void LoadSettingsForSubject(IWithId subject)
      {
         // no settings
      }

      public abstract string PresentationKey { get; }

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         updateOriginText();
      }

      private bool canHandle(AnalysableEvent analysableEvent)
      {
         return Equals(PopulationDataCollector, analysableEvent.Analysable);
      }

   }
}