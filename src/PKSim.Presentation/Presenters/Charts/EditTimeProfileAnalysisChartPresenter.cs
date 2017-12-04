using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Binders;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Charts;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IEditTimeProfileAnalysisChartPresenter : IEditPopulationAnalysisChartPresenter, IPKAnalysisWithChartPresenter,
      IListener<ObservedDataRemovedFromAnalysableEvent>
   {
      void AddObservedData(IReadOnlyList<DataRepository> observedData);
   }

   public class EditTimeProfileAnalysisChartPresenter : EditPopulationAnalysisChartPresenter<TimeProfileAnalysisChart, TimeProfileXValue, TimeProfileYValue>, IEditTimeProfileAnalysisChartPresenter
   {
      private readonly IColorGenerator _colorGenerator;
      private readonly IObservedDataTask _observedDataTask;
      private readonly IPopulationPKAnalysisPresenter _pkAnalysisPresenter;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ObservedDataDragDropBinder _observedDataDragDropBinder;
      private readonly IEditTimeProfileAnalysisChartView _timeProfileAnalysisChartView;
      private readonly IPresentationSettingsTask _presentationSettingsTask;
      private ChartDisplayMode _chartDisplayMode;
      private const string CHART_DISPLAY_MODE_SETTING = "chartDisplayMode";

      public EditTimeProfileAnalysisChartPresenter(IEditTimeProfileAnalysisChartView view, ITimeProfileChartPresenter timeProfileChartPresenter, ITimeProfileChartDataCreator timeProfileChartDataCreator, IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter, IPopulationAnalysisTask populationAnalysisTask, IColorGenerator colorGenerator, IObservedDataTask observedDataTask, IPopulationPKAnalysisPresenter pkAnalysisPresenter, IDimensionRepository dimensionRepository, IPresentationSettingsTask presentationSettingsTask)
         : base(view, timeProfileChartPresenter, timeProfileChartDataCreator, populationSimulationAnalysisStarter, populationAnalysisTask, ApplicationIcons.TimeProfileAnalysis)
      {
         _colorGenerator = colorGenerator;
         _observedDataTask = observedDataTask;
         _pkAnalysisPresenter = pkAnalysisPresenter;
         _dimensionRepository = dimensionRepository;
         _presentationSettingsTask = presentationSettingsTask;
         _timeProfileAnalysisChartView = view;
         timeProfileChartPresenter.DragDrop += OnDragDrop;
         timeProfileChartPresenter.DragOver += OnDragOver;
         timeProfileChartPresenter.ObservedDataSettingsChanged += RefreshData;
         _chartDisplayMode = ChartDisplayMode.Chart;
         _observedDataDragDropBinder = new ObservedDataDragDropBinder();
         _timeProfileAnalysisChartView.SetChartView(_populationAnalysisChartPresenter.BaseView);
         _timeProfileAnalysisChartView.SetPKAnalysisView(_pkAnalysisPresenter.BaseView);
      }

      protected override void RefreshData()
      {
         base.RefreshData();
         if (_chartDisplayMode == ChartDisplayMode.PKAnalysis)
            calculatePKAnalysis();
      }

      protected override ChartData<TimeProfileXValue, TimeProfileYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(PopulationAnalysisChart, AggregationFunctions.QuantityAggregation);
      }

      public virtual void OnDragOver(object sender, DragEventArgs e)
      {
         _observedDataDragDropBinder.PrepareDrag(e);
      }

      public virtual void OnDragDrop(object sender, DragEventArgs e)
      {
         var droppedObservedData = _observedDataDragDropBinder.DroppedObservedDataFrom(e);
         AddObservedData(droppedObservedData.ToList());
         RefreshData();
      }

      public void AddObservedData(IReadOnlyList<DataRepository> observedData)
      {
         var observedDataToAdd = observedData.Where(x => !PopulationAnalysisChart.UsesObservedData(x)).ToList();

         if (!observedDataToAdd.Any())
            return;

         checkDimensionCompatibilityFor(observedData);

         observedDataToAdd.Each(PopulationAnalysisChart.AddObservedData);

         _observedDataTask.AddObservedDataToAnalysable(observedData, PopulationDataCollector);

         observedData.SelectMany(x => x.ObservationColumns()).Each(c =>
         {
            var curveOption = PopulationAnalysisChart.CurveOptionsFor(c);
            curveOption.Color = _colorGenerator.NextColor();
         });
      }

      private void checkDimensionCompatibilityFor(IReadOnlyList<DataRepository> observedData)
      {
         //use merge dimension for the output to ensure proper conversion between mass and amount
         var availableDimensions = PopulationAnalysisChart.PopulationAnalysis.All<PopulationAnalysisOutputField>()
            .Select(f => _dimensionRepository.MergedDimensionFor(new NumericFieldContext(f, PopulationDataCollector)))
            .Distinct()
            .ToList();

         var availableDimensionNames = availableDimensions.Select(x => x.DisplayName).ToList();

         foreach (var dataRepository in observedData)
         {
            var allUsedDimensions = dataRepository.ObservationColumns().Select(x => x.Dimension)
               .Distinct()
               .ToList();

            if (allUsedDimensions.Any(dimension => availableDimensions.Any(d => !d.CanConvertToUnit(dimension.DefaultUnitName))))
               throw new PKSimException(PKSimConstants.Error.CannotAddObservedDataBecauseOfDimensionMismatch(dataRepository.Name,
                  availableDimensionNames,
                  allUsedDimensions.Select(x => x.DisplayName)));
         }
      }

      public void Handle(ObservedDataRemovedFromAnalysableEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         eventToHandle.ObservedData.Each(PopulationAnalysisChart.RemoveObservedData);
         RefreshData();
      }

      public override void LoadSettingsForSubject(IWithId subject)
      {
         base.LoadSettingsForSubject(subject);
         _pkAnalysisPresenter.LoadSettingsForSubject(subject);
         _settings = _presentationSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(this, subject);

         setViewMode(_settings.IsEqual(CHART_DISPLAY_MODE_SETTING, ChartDisplayMode.PKAnalysis));
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditTimeProfileAnalysisChartPresenter;

      public void SwitchPKAnalysisPlot()
      {
         setViewMode(_chartDisplayMode == ChartDisplayMode.Chart);
         _settings.SetSetting(CHART_DISPLAY_MODE_SETTING, _chartDisplayMode);
      }

      private void setViewMode(bool shouldShowAnalysis)
      {
         if (shouldShowAnalysis)
            showAnalysis();
         else
            showChart();
      }

      private void showChart()
      {
         _chartDisplayMode = ChartDisplayMode.Chart;
         _timeProfileAnalysisChartView.ShowChartView();
      }

      private void showAnalysis()
      {
         _chartDisplayMode = ChartDisplayMode.PKAnalysis;
         calculatePKAnalysis();
         _timeProfileAnalysisChartView.ShowPKAnalysisView();
      }

      private void calculatePKAnalysis()
      {
         var chartData = CreateChartData();
         _pkAnalysisPresenter.CalculatePKAnalysis(PopulationDataCollector, chartData);
      }

      private bool canHandle(AnalysableEvent analysableEvent)
      {
         return Equals(analysableEvent.Analysable, PopulationDataCollector);
      }
   }
}