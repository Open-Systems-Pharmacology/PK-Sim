using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Charts;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditTimeProfileAnalysisChartPresenter : ContextSpecification<EditTimeProfileAnalysisChartPresenter>
   {
      protected IEditTimeProfileAnalysisChartView _view;
      protected ITimeProfileChartPresenter _timeProfilerChartPresenter;
      protected ITimeProfileChartDataCreator _timeProfileChartDataCreator;
      protected IPopulationSimulationAnalysisStarter _populationSimulationAnalysisStarter;
      protected IPopulationAnalysisTask _populationAnalysisTask;
      protected IColorGenerator _colorGenerator;
      protected IObservedDataTask _observedDataTask;
      protected IPopulationPKAnalysisPresenter _pkAnalysisPresenter;
      protected TimeProfileAnalysisChart _timeProfileAnalysisChart;
      protected IPopulationDataCollector _populationDataCollector;
      protected ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      protected DataRepository _observedDataRepository;
      protected IDragEvent _dragEventArgs;
      protected PaneData<TimeProfileXValue, TimeProfileYValue> _paneData;
      protected PopulationStatisticalAnalysis _populationStatisticalAnalysis;
      protected IDimensionRepository _dimensionRepository;
      protected IPresentationSettingsTask _presenterSettingsTask;
      protected IPKAnalysesTask _pkAnalysesTask;
      protected IStatisticalDataCalculator _statisticalDataCalculator;
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _view = A.Fake<IEditTimeProfileAnalysisChartView>();
         _timeProfilerChartPresenter = A.Fake<ITimeProfileChartPresenter>();
         _timeProfileChartDataCreator = A.Fake<ITimeProfileChartDataCreator>();
         _populationSimulationAnalysisStarter = A.Fake<IPopulationSimulationAnalysisStarter>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();
         _colorGenerator = A.Fake<IColorGenerator>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _pkAnalysisPresenter = A.Fake<IPopulationPKAnalysisPresenter>();
         _dimensionRepository = A.Fake<IDimensionRepository>();

         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         _pkAnalysesTask = A.Fake<IPKAnalysesTask>();
         _statisticalDataCalculator = new StatisticalDataCalculator();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new EditTimeProfileAnalysisChartPresenter(_view, _timeProfilerChartPresenter, _timeProfileChartDataCreator,
            _populationSimulationAnalysisStarter, _populationAnalysisTask, _colorGenerator, _observedDataTask, _pkAnalysisPresenter, _dimensionRepository, _presenterSettingsTask, _representationInfoRepository, _statisticalDataCalculator, _pkAnalysesTask);

         _timeProfileAnalysisChart = new TimeProfileAnalysisChart();
         _populationStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _timeProfileAnalysisChart.PopulationAnalysis = _populationStatisticalAnalysis;
         
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         sut.InitializeAnalysis(_timeProfileAnalysisChart, _populationDataCollector);

         _observedDataRepository = DomainHelperForSpecs.ObservedData();
         _dragEventArgs = A.Fake<IDragEvent>();
         A.CallTo(() => _dragEventArgs.Data<IEnumerable<ITreeNode>>()).Returns(new List<ITreeNode> { new ObservedDataNode(new ClassifiableObservedData { Subject = _observedDataRepository }) });
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(null, null);
         var concentrationDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         var yAxis = new AxisData(concentrationDimension, concentrationDimension.DefaultUnit, Scalings.Linear);
         _paneData = new PaneData<TimeProfileXValue, TimeProfileYValue>(yAxis);
         _chartData.AddPane(_paneData);
         A.CallTo(_timeProfileChartDataCreator).WithReturnType<ChartData<TimeProfileXValue, TimeProfileYValue>>().Returns(_chartData);

         var outputField = new PopulationAnalysisOutputField { Dimension = DomainHelperForSpecs.MassConcentrationDimensionForSpecs() };
         _populationStatisticalAnalysis.Add(outputField);

         A.CallTo(() => _dimensionRepository.MergedDimensionFor(A<NumericFieldContext>._)).Returns(concentrationDimension);
      }
   }

   public class When_the_time_profile_presenter_is_being_notified_that_observed_data_where_dropped : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Because()
      {
         sut.OnDragDrop(_timeProfilerChartPresenter, _dragEventArgs);
      }

      [Observation]
      public void should_add_the_observed_data_to_the_underlying_time_profile_chart()
      {
         _timeProfileAnalysisChart.AllObservedData().ShouldContain(_observedDataRepository);
      }


      [Observation]
      public void should_refresh_the_chart()
      {
         A.CallTo(() => _timeProfilerChartPresenter.Show(_chartData, _timeProfileAnalysisChart)).MustHaveHappened();
      }
   }

   public class When_notified_that_observed_data_were_removed_for_the_current_population_data_collector : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Context()
      {
         base.Context();
         _timeProfileAnalysisChart.AddObservedData(_observedDataRepository);
      }

      protected override void Because()
      {
         sut.Handle(new ObservedDataRemovedFromAnalysableEvent(_populationDataCollector, _observedDataRepository));
      }

      [Observation]
      public void should_remove_the_observed_data_from_the_analysis()
      {
         _timeProfileAnalysisChart.UsesObservedData(_observedDataRepository).ShouldBeFalse();
      }
   }

   public class When_notified_that_observed_data_were_removed_for_another_population_data_collector : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Context()
      {
         base.Context();
         _timeProfileAnalysisChart.AddObservedData(_observedDataRepository);
      }

      protected override void Because()
      {
         sut.Handle(new ObservedDataRemovedFromAnalysableEvent(A.Fake<IAnalysable>(), _observedDataRepository));
      }

      [Observation]
      public void should_not_remove_the_observed_data_from_the_analysis()
      {
         _timeProfileAnalysisChart.UsesObservedData(_observedDataRepository).ShouldBeTrue();
      }
   }

   public class When_the_time_profile_presenter_is_displaying_the_pk_analyses : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Because()
      {
         sut.SwitchPKAnalysisPlot();
      }

      [Observation]
      public void should_calculate_the_pk_analysis_for_the_current_chart_data()
      {
         A.CallTo(() => _pkAnalysisPresenter.CalculatePKAnalysisOnCurves(_populationDataCollector, _chartData)).MustHaveHappened();
      }

      [Observation]
      public void should_show_the_pk_analysis_view_in_the_view()
      {
         A.CallTo(() => _view.SetPKAnalysisView(_pkAnalysisPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_time_profile_presenter_is_switching_from_chart_to_pk_analyses_and_back_to_chart : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Because()
      {
         sut.SwitchPKAnalysisPlot();
         sut.SwitchPKAnalysisPlot();
      }

      [Observation]
      public void should_display_the_chart_view_again()
      {
         //2 times: start and switch
         A.CallTo(() => _view.SetChartView(_timeProfilerChartPresenter.BaseView)).MustHaveHappenedTwiceExactly();
      }
   }

   public class when_loading_settings_and_the_settings_indicate_that_the_chart_view_should_be_shown : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      private DefaultPresentationSettings _settings;

      protected override void Context()
      {
         base.Context();
         _settings = A.Fake<DefaultPresentationSettings>();
         A.CallTo(_settings).WithReturnType<ChartDisplayMode>().Returns(ChartDisplayMode.Chart);
      }

      protected override void Because()
      {
         sut.LoadSettingsForSubject(A.Fake<IWithId>());
      }

      [Observation]
      public void should_make_sure_that_the_chart_view_is_showing()
      {
         A.CallTo(() => _view.SetChartView(_timeProfilerChartPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class when_loading_settings_and_the_settings_indicate_that_the_analysis_view_should_be_shown : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      private DefaultPresentationSettings _settings;

      protected override void Context()
      {
         base.Context();
         _settings = A.Fake<DefaultPresentationSettings>();
         A.CallTo(() => _presenterSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(A<IPresenterWithSettings>._, A<IWithId>._)).Returns(_settings);
         A.CallTo(_settings).WithReturnType<ChartDisplayMode>().Returns(ChartDisplayMode.PKAnalysis);
      }

      protected override void Because()
      {
         sut.LoadSettingsForSubject(A.Fake<IWithId>());
      }

      [Observation]
      public void should_make_sure_that_the_analysis_view_is_showing()
      {
         A.CallTo(() => _view.SetPKAnalysisView(_pkAnalysisPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_updating_the_data_source_on_presenter : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.SwitchPKAnalysisPlot();
      }

      protected override void Because()
      {
         sut.UpdateAnalysisBasedOn(A.Fake<IPopulationDataCollector>());
      }

      [Observation]
      public void the_analysis_should_be_updated_at_the_same_time()
      {
         A.CallTo(() => _pkAnalysisPresenter.CalculatePKAnalysisOnCurves(A<IPopulationDataCollector>._, A<ChartData<TimeProfileXValue, TimeProfileYValue>>._)).MustHaveHappened();
      }
   }

   public class When_adding_some_observed_data_to_the_analysis_whose_dimension_does_not_match_the_dimension_of_the_output_being_displayed : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Context()
      {
         base.Context();
         _observedDataRepository.ObservationColumns().Each(col => { col.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs(); });
      }

      [Observation]
      public void should_notify_the_user_that_the_action_cannot_be_performed()
      {
         The.Action(() => sut.AddObservedData(new[] { _observedDataRepository })).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_aggregating_pk_parameters_from_individuals : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      class TestQuantityPKParameter : QuantityPKParameter
      {
         public float[] _values { get; set; }

         public override float[] ValuesAsArray { get => _values; }
      }

      class TestEditTimeProfileAnalysisChartPresenter : EditTimeProfileAnalysisChartPresenter
      {
         public TestEditTimeProfileAnalysisChartPresenter(
               IEditTimeProfileAnalysisChartView view,
            ITimeProfileChartPresenter timeProfileChartPresenter,
            ITimeProfileChartDataCreator timeProfileChartDataCreator,
            IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter,
            IPopulationAnalysisTask populationAnalysisTask,
            IColorGenerator colorGenerator,
            IObservedDataTask observedDataTask,
            IPopulationPKAnalysisPresenter pkAnalysisPresenter,
            IDimensionRepository dimensionRepository,
            IPresentationSettingsTask presentationSettingsTask,
            IRepresentationInfoRepository representationInfoRepository,
            IStatisticalDataCalculator statisticalDataCalculator,
            IPKAnalysesTask pKAnalysesTask) : base (view, timeProfileChartPresenter, timeProfileChartDataCreator, populationSimulationAnalysisStarter,
               populationAnalysisTask, colorGenerator, observedDataTask, pkAnalysisPresenter, dimensionRepository, presentationSettingsTask,
               representationInfoRepository, statisticalDataCalculator, pKAnalysesTask)
         { }

         public IEnumerable<PopulationPKAnalysis> AggregatePKAnalysis(IPopulationDataCollector populationDataCollector, IEnumerable<QuantityPKParameter> pkParameters, string captionPrefix)
         {
            return aggregatePKAnalysis(populationDataCollector, pkParameters, captionPrefix);
         }
      }

      private IEnumerable<PopulationPKAnalysis> _pkAnalyses;
      private PopulationSimulation _simulation;
      protected PercentileStatisticalAggregation _percentileStatisticalAggregation;
      protected const string _percentileId = "Percentile";

      protected override void Context()
      {
         base.Context();

         _percentileStatisticalAggregation = new PercentileStatisticalAggregation { Selected = true, Percentile = 50 };
         _populationStatisticalAnalysis.AddStatistic(_percentileStatisticalAggregation);
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_percentileStatisticalAggregation)).Returns(_percentileId);
         sut = new TestEditTimeProfileAnalysisChartPresenter(_view, _timeProfilerChartPresenter, _timeProfileChartDataCreator,
            _populationSimulationAnalysisStarter, _populationAnalysisTask, _colorGenerator, _observedDataTask, _pkAnalysisPresenter, _dimensionRepository, _presenterSettingsTask, _representationInfoRepository, _statisticalDataCalculator, _pkAnalysesTask);

         var model = A.Fake<IModel>();
         A.CallTo(() => model.MoleculeNameFor("Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)")).Returns("Esomeprazole");
         A.CallTo(() => model.MoleculeNameFor("Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)")).Returns("Esomeprazole-2");
         _simulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _simulation.Model).Returns(model);
         A.CallTo(() => _simulation.Compounds).Returns(new[] { new Compound() { Name = "Esomeprazole" }, new Compound() { Name = "Esomeprazole-2" } });
         _simulation.AddCompoundPK(new CompoundPK() { CompoundName = "Esomeprazol-2" });
         var analysis = new TimeProfileAnalysisChart();
         analysis.PopulationAnalysis = _populationStatisticalAnalysis;
         sut.InitializeAnalysis(analysis, _simulation);
      }

      protected override void Because()
      {
         var pkParameters = new[]
         {
            new TestQuantityPKParameter() { Name = "Name 1", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   _values = new[] { 0.000f, 0.050f, 0.025f, 0.075f, 1.000f } },
            new TestQuantityPKParameter() { Name = "Name 2", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   _values = new[] { 0.00f,  0.25f,  0.75f,  0.50f,  1.00f  } },
            new TestQuantityPKParameter() { Name = "Name 3", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)",   _values = new[] { 0.0f,   2.5f,   5.0f,   7.5f,   10.0f  } },
            new TestQuantityPKParameter() { Name = "Name 1", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", _values = new[] { 0f,     0f,     0f,     0f,     0f } },
            new TestQuantityPKParameter() { Name = "Name 2", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", _values = new[] { 0f,     0f,     0f,     0f,     0f } },
            new TestQuantityPKParameter() { Name = "Name 3", QuantityPath = "Organism|PeripheralVenousBlood|Esomeprazole-2|Plasma (Peripheral Venous Blood)", _values = new[] { 0f,     0f,     0f,     0f,     0f } }
         };

         _pkAnalyses = (sut as TestEditTimeProfileAnalysisChartPresenter).AggregatePKAnalysis(_simulation, pkParameters, "Esomeprazole");
      }

      [Observation]
      public void should_aggregate_correctly()
      {
         _pkAnalyses.ShouldNotBeEmpty();
         A.CallTo(() => _pkAnalysesTask.CreatePKAnalysisFromValues(
            A<PKValues>.That.Matches(p => p.Values.ContainsItem(0.05f) && p.Values.ContainsItem(0.5f) && p.Values.ContainsItem(5f)),
            A<Simulation>.Ignored,
            A<Compound>.Ignored
         )).MustHaveHappened();
         _pkAnalyses.Count().ShouldBeEqualTo(1);
         var curveData = _pkAnalyses.First().CurveData;
         curveData.Caption.ShouldBeEqualTo("Esomeprazole-Percentile");
         curveData.QuantityPath.ShouldBeEqualTo("Organism|PeripheralVenousBlood|Esomeprazole|Plasma (Peripheral Venous Blood)");
      }
   }
}