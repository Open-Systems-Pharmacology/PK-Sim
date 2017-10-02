using System.Collections.Generic;
using System.Windows.Forms;
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
   public abstract class concern_for_EditTimeProfileAnalysisChartPresenter : ContextSpecification<IEditTimeProfileAnalysisChartPresenter>
   {
      protected IEditTimeProfileAnalysisChartView _view;
      protected ITimeProfileChartPresenter _timeProfilerChartPresenter;
      private ITimeProfileChartDataCreator _timeProfileChartDataCreator;
      private IPopulationSimulationAnalysisStarter _populationSimulationAnalysisStarter;
      private IPopulationAnalysisTask _populationAnalysisTask;
      private IColorGenerator _colorGenerator;
      protected IObservedDataTask _observedDataTask;
      protected IPopulationPKAnalysisPresenter _pkAnalysisPresenter;
      protected TimeProfileAnalysisChart _timeProfileAnalysisChart;
      protected IPopulationDataCollector _populationDataCollector;
      protected ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      protected DataRepository _observedDataRepository;
      protected DragEventArgs _dragEventArgs;
      private PaneData<TimeProfileXValue, TimeProfileYValue> _paneData;
      private PopulationStatisticalAnalysis _populationStatisticalAnalysis;
      private IDimensionRepository _dimensionRepository;
      protected IPresentationSettingsTask _presenterSettingsTask;

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
         sut = new EditTimeProfileAnalysisChartPresenter(_view, _timeProfilerChartPresenter, _timeProfileChartDataCreator,
            _populationSimulationAnalysisStarter, _populationAnalysisTask, _colorGenerator, _observedDataTask, _pkAnalysisPresenter, _dimensionRepository, _presenterSettingsTask);

         _timeProfileAnalysisChart = new TimeProfileAnalysisChart();
         _populationStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _timeProfileAnalysisChart.PopulationAnalysis = _populationStatisticalAnalysis;
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         sut.InitializeAnalysis(_timeProfileAnalysisChart, _populationDataCollector);

         _observedDataRepository = DomainHelperForSpecs.ObservedData();
         var data = new DragDropInfo(
            new List<ITreeNode> {new ObservedDataNode(new ClassifiableObservedData {Subject = _observedDataRepository})}
         );

         _dragEventArgs = new DragEventArgs(new DataObject(data), 0, 0, 0, DragDropEffects.All, DragDropEffects.All);

         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(null, null);
         var concentrationDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         var yAxis = new AxisData(concentrationDimension, concentrationDimension.DefaultUnit, Scalings.Linear);
         _paneData = new PaneData<TimeProfileXValue, TimeProfileYValue>(yAxis);
         _chartData.AddPane(_paneData);
         A.CallTo(_timeProfileChartDataCreator).WithReturnType<ChartData<TimeProfileXValue, TimeProfileYValue>>().Returns(_chartData);

         var outputField = new PopulationAnalysisOutputField {Dimension = DomainHelperForSpecs.MassConcentrationDimensionForSpecs()};
         _populationStatisticalAnalysis.Add(outputField);

         A.CallTo(() => _dimensionRepository.MergedDimensionFor(A<NumericFieldContext>._)).Returns(concentrationDimension);
      }
   }

   public class When_the_time_profile_presenter_is_being_notified_that_observed_data_where_dropped : concern_for_EditTimeProfileAnalysisChartPresenter
   {
      protected override void Because()
      {
         _timeProfilerChartPresenter.DragDrop += Raise.FreeForm.With(_timeProfilerChartPresenter, _dragEventArgs);
      }

      [Observation]
      public void should_add_the_observed_data_to_the_underlying_time_profile_chart()
      {
         _timeProfileAnalysisChart.AllObservedData().ShouldContain(_observedDataRepository);
      }

      [Observation]
      public void should_add_the_observed_data_as_used_by_the_population_data_collector()
      {
         A.CallTo(() => _observedDataTask.AddObservedDataToAnalysable(A<IReadOnlyList<DataRepository>>._, _populationDataCollector)).MustHaveHappened();
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
         A.CallTo(() => _pkAnalysisPresenter.CalculatePKAnalysis(_populationDataCollector, _chartData)).MustHaveHappened();
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
         A.CallTo(() => _view.SetChartView(_timeProfilerChartPresenter.BaseView)).MustHaveHappened(Repeated.Exactly.Twice);
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
         A.CallTo(() => _pkAnalysisPresenter.CalculatePKAnalysis(A<IPopulationDataCollector>._, A<ChartData<TimeProfileXValue, TimeProfileYValue>>._)).MustHaveHappened();
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
         The.Action(() => sut.AddObservedData(new[] {_observedDataRepository})).ShouldThrowAn<PKSimException>();
      }
   }
}