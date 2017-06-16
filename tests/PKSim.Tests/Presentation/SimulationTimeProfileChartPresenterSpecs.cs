using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Charts;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using OSPSuite.Presentation.Settings;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationTimeProfileChartPresenter : ContextSpecification<ISimulationTimeProfileChartPresenter>
   {
      protected ISimulationTimeProfileChartView _view;
      protected IChartDisplayPresenter _chartDisplayPresenter;
      protected IChartEditorPresenter _chartEditorPresenter;
      protected IDataColumnToPathElementsMapper _dataColumnToPathElementsMapper;
      protected IIndividualPKAnalysisPresenter _pkAnalysisPresenter;
      protected IChartTask _chartTask;
      protected IObservedDataTask _observedDataTask;
      protected IChartEditorAndDisplayPresenter _chartEditorAndDisplayPresenter;
      protected IList<ChartEditorLayoutTemplate> _allTemplates;
      protected IStartOptions _runOptions;
      protected IChartEditorLayoutTask _chartLayoutTask;
      protected IChartTemplatingTask _chartTemplatingTask;
      private IProjectRetriever _projectRetriever;
      private ChartPresenterContext _chartPresenterContext;
      private ICurveNamer _curveNamer;
      private IChartUpdater _chartUpdateTask;

      protected override void Context()
      {
         _view = A.Fake<ISimulationTimeProfileChartView>();
         _chartDisplayPresenter = A.Fake<IChartDisplayPresenter>();
         _pkAnalysisPresenter = A.Fake<IIndividualPKAnalysisPresenter>();
         _chartEditorPresenter = A.Fake<IChartEditorPresenter>();
         _chartEditorAndDisplayPresenter = A.Fake<IChartEditorAndDisplayPresenter>();
         _dataColumnToPathElementsMapper = A.Fake<IDataColumnToPathElementsMapper>();
         _chartTask = A.Fake<IChartTask>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _chartLayoutTask = A.Fake<IChartEditorLayoutTask>();
         _chartUpdateTask = A.Fake<IChartUpdater>();
         _allTemplates = new List<ChartEditorLayoutTemplate>();
         A.CallTo(() => _chartLayoutTask.AllTemplates()).Returns(_allTemplates);
         A.CallTo(() => _chartEditorAndDisplayPresenter.EditorPresenter).Returns(_chartEditorPresenter);
         A.CallTo(() => _chartEditorAndDisplayPresenter.DisplayPresenter).Returns(_chartDisplayPresenter);
         A.CallTo(() => _chartEditorPresenter.DataBrowserColumnSettingsFor(A<BrowserColumns>._)).Returns(new GridColumnSettings(BrowserColumns.Origin.ToString()));
         A.CallTo(() => _chartEditorPresenter.AxisOptionsColumnSettingsFor(A<AxisOptionsColumns>._)).Returns(new GridColumnSettings(AxisOptionsColumns.AxisType.ToString()));
         A.CallTo(() => _chartEditorPresenter.CurveOptionsColumnSettingsFor(A<CurveOptionsColumns>._)).Returns(new GridColumnSettings(CurveOptionsColumns.xData.ToString()));
         _chartTemplatingTask = A.Fake<IChartTemplatingTask>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _chartPresenterContext = A.Fake<ChartPresenterContext>();
         _curveNamer = A.Fake<ICurveNamer>();

         A.CallTo(() => _chartPresenterContext.ChartEditorAndDisplayPresenter).Returns(_chartEditorAndDisplayPresenter);
         A.CallTo(() => _chartPresenterContext.CurveNamer).Returns(_curveNamer);
         A.CallTo(() => _chartPresenterContext.EditorLayoutTask).Returns(_chartLayoutTask);
         A.CallTo(() => _chartPresenterContext.TemplatingTask).Returns(_chartTemplatingTask);
         A.CallTo(() => _chartPresenterContext.ProjectRetriever).Returns(_projectRetriever);

         sut = new SimulationTimeProfileChartPresenter(_view, _chartPresenterContext, _pkAnalysisPresenter, _chartTask, _observedDataTask, _chartTemplatingTask, _chartUpdateTask);
      }

   }

   public class When_handling_simulation_result_updates : concern_for_SimulationTimeProfileChartPresenter
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {Name = "SimulationName", DataRepository = new DataRepository(), SimulationSettings = new SimulationSettings()};
         sut.UpdateAnalysisBasedOn(_simulation);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_result_in_update_to_the_chart_origin_text()
      {
         A.CallTo(() => _chartTask.SetOriginTextFor(_simulation.Name, sut.Chart)).MustHaveHappened();
      }
   }

   public class When_retrieving_the_default_template_from_a_simulation_with_default_template : concern_for_SimulationTimeProfileChartPresenter
   {
      private CurveChartTemplate _defaultChartTemplate;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {DataRepository = new DataRepository(), SimulationSettings = new SimulationSettings()};
         _defaultChartTemplate = new CurveChartTemplate {IsDefault = true, Name = "one"};
         _simulation.SimulationSettings.AddChartTemplate(_defaultChartTemplate);
         _simulation.SimulationSettings.AddChartTemplate(new CurveChartTemplate {Name = "two"});
         _simulation.SimulationSettings.AddChartTemplate(new CurveChartTemplate {Name = "three"});
         sut.InitializeAnalysis(new SimulationTimeProfileChart());
      }

      protected override void Because()
      {
         sut.UpdateAnalysisBasedOn(_simulation);
      }

      [Observation]
      public void should_use_the_default_tempalte()
      {
         A.CallTo(() => _chartTemplatingTask.InitFromTemplate(
            A<CurveChart>._, A<IChartEditorAndDisplayPresenter>._,
            A<IReadOnlyCollection<DataColumn>>._,
            A<IReadOnlyCollection<IndividualSimulation>>._,
            A<Func<DataColumn, string>>._, _defaultChartTemplate)).MustHaveHappened();
      }
   }

   public class When_retrieving_the_default_template_from_a_simulation_with_templates_but_no_default : concern_for_SimulationTimeProfileChartPresenter
   {
      private CurveChartTemplate _defaultChartTemplate;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         _simulation = new IndividualSimulation {DataRepository = new DataRepository(), SimulationSettings = new SimulationSettings()};
         _defaultChartTemplate = new CurveChartTemplate {IsDefault = false};
         _simulation.SimulationSettings.AddChartTemplate(_defaultChartTemplate);
         base.Context();
         sut.InitializeAnalysis(new SimulationTimeProfileChart());
      }

      protected override void Because()
      {
         sut.UpdateAnalysisBasedOn(_simulation);
      }

      [Observation]
      public void should_use_the_first_template_in_the_sequence()
      {
         A.CallTo(() => _chartTemplatingTask.InitFromTemplate(
            A<CurveChart>._, A<IChartEditorAndDisplayPresenter>._,
            A<IReadOnlyCollection<DataColumn>>._,
            A<IReadOnlyCollection<IndividualSimulation>>._,
            A<Func<DataColumn, string>>._, _defaultChartTemplate)).MustHaveHappened();
      }
   }

   public class When_handling_template_change_events : concern_for_SimulationTimeProfileChartPresenter
   {
      private Simulation _simulation;

      protected override void Context()
      {
         _simulation = new IndividualSimulation {DataRepository = new DataRepository()};
         base.Context();
         sut.UpdateAnalysisBasedOn(_simulation);
      }

      protected override void Because()
      {
         sut.Handle(new ChartTemplatesChangedEvent(_simulation));
      }

      [Observation]
      public void the_chart_editor_should_have_reset_the_template_menus()
      {
         A.CallTo(() => _chartEditorPresenter.AddChartTemplateMenu(_simulation, A<Action<CurveChartTemplate>>._)).MustHaveHappened();
      }
   }

   public class When_the_concentration_chart_presenter_is_being_created : concern_for_SimulationTimeProfileChartPresenter
   {
      private ChartEditorLayoutTemplate _template1;
      private ChartEditorLayoutTemplate _template2;

      protected override void Context()
      {
         base.Context();
         _template1 = new ChartEditorLayoutTemplate();
         _template2 = new ChartEditorLayoutTemplate();
         _allTemplates.Add(_template1);
         _allTemplates.Add(_template2);
      }

      [Observation]
      public void should_set_the_chart_display_control_in_its_view()
      {
         A.CallTo(() => _view.SetChartView(_chartEditorAndDisplayPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_chart_is_being_set_into_the_chart_presenter_and_the_chart_should_be_loaded_from_the_template : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private DataRepository _observedData1;
      private DataRepository _observedData2;
      private List<DataRepository> _allDataRepositoryAdded;

      protected override void Context()
      {
         base.Context();
         _chart = new SimulationTimeProfileChart();
         _observedData1 = new DataRepository();
         _observedData2 = new DataRepository();
         _chart.AddObservedData(_observedData1);
         _chart.AddObservedData(_observedData2);

         A.CallTo(() => _chartEditorPresenter.AddDataRepositories(A<IEnumerable<DataRepository>>._))
            .Invokes(x => _allDataRepositoryAdded = x.GetArgument<IEnumerable<DataRepository>>(0).ToList());
      }

      protected override void Because()
      {
         sut.InitializeAnalysis(_chart);
      }

      [Observation]
      public void should_add_the_available_observed_data_in_the_chart_editor()
      {
         _allDataRepositoryAdded.ShouldContain(_observedData1, _observedData2);
      }
   }

   public class When_the_chart_is_being_set_into_the_chart_presenter_and_the_chart_should_not_be_loaded_from_the_template : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private DataRepository _observedData1;
      private DataRepository _observedData2;
      private List<DataRepository> _allDataRepositoryAdded;

      protected override void Context()
      {
         base.Context();
         _chart = new SimulationTimeProfileChart();
         _observedData1 = new DataRepository();
         _observedData2 = new DataRepository();
         _chart.AddObservedData(_observedData1);
         _chart.AddObservedData(_observedData2);

         A.CallTo(() => _chartEditorPresenter.AddDataRepositories(A<IEnumerable<DataRepository>>._))
            .Invokes(x => _allDataRepositoryAdded = x.GetArgument<IEnumerable<DataRepository>>(0).ToList());
      }

      protected override void Because()
      {
         sut.InitializeAnalysis(_chart);
      }

      [Observation]
      public void should_set_the_chart_as_data_source_into_the_the_chart_display_presenter()
      {
         sut.Chart.ShouldBeEqualTo(_chart);
      }

      [Observation]
      public void should_add_the_available_observed_data_in_the_chart_editor()
      {
         _allDataRepositoryAdded.ShouldContain(_observedData1, _observedData2);
      }
   }

   public class When_the_concentration_chart_presenter_is_told_to_clear_its_data : concern_for_SimulationTimeProfileChartPresenter
   {
      private DataRepository _dataRepository;
      private List<DataRepository> _allDataRepositoryRemoved;

      protected override void Context()
      {
         base.Context();
         _dataRepository = A.Fake<DataRepository>();
         var simulation = A.Fake<IndividualSimulation>();
         simulation.DataRepository = _dataRepository;
         sut.UpdateAnalysisBasedOn(simulation);

         _allDataRepositoryRemoved  =new List<DataRepository>();
         A.CallTo(() => _chartEditorPresenter.RemoveDataRepositories(A<IEnumerable<DataRepository>>._))
            .Invokes(x => _allDataRepositoryRemoved.AddRange(x.GetArgument<IEnumerable<DataRepository>>(0)));
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_not_remove_the_data_repository_from_the_chart_editor_presenter()
      {
         _allDataRepositoryRemoved.ShouldNotContain(_dataRepository);
      }

      [Observation]
      public void should_clart_data_source_into_the_chart_display_presenter_and_chart_editor_presenter()
      {
         A.CallTo(() => _chartDisplayPresenter.Clear()).MustHaveHappened();
         A.CallTo(() => _chartEditorPresenter.Clear()).MustHaveHappened();
      }
   }

   public class When_notified_that_curve_data_has_been_hidden : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private Curve _curve;

      protected override void Context()
      {
         base.Context();
         _chart = new SimulationTimeProfileChart();
         _curve = new Curve();
         _curve.SetxData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());
         _curve.SetyData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());
         _chart.AddCurve(_curve);
         sut.InitializeAnalysis(_chart);
         sut.UpdateAnalysisBasedOn(A.Fake<IndividualSimulation>());
         sut.SwitchPKAnalysisPlot();
      }

      protected override void Because()
      {
         _curve.Visible = false;
      }

      [Observation]
      public void the_pk_analysis_presenter_must_also_be_updated_at_the_same_time()
      {
         A.CallTo(() => _pkAnalysisPresenter.ShowPKAnalysis(A<IEnumerable<Simulation>>._, A<IEnumerable<Curve>>._)).MustHaveHappened();
      }
   }

   public class When_notified_that_curve_name_has_changed : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private Curve _curve;

      protected override void Context()
      {
         base.Context();
         _chart = new SimulationTimeProfileChart();
         _curve = new Curve();
         _curve.SetxData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());
         _curve.SetyData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());
         _chart.AddCurve(_curve);
         sut.InitializeAnalysis(_chart);
         sut.UpdateAnalysisBasedOn(A.Fake<IndividualSimulation>());
         sut.SwitchPKAnalysisPlot();
      }

      protected override void Because()
      {
         _curve.Name = "XXX";
      }

      [Observation]
      public void the_pk_analysis_presenter_must_also_be_updated_at_the_same_time()
      {
         A.CallTo(() => _pkAnalysisPresenter.ShowPKAnalysis(A<IEnumerable<Simulation>>._, A<IEnumerable<Curve>>._)).MustHaveHappened();
      }
   }

   public class When_notified_that_observed_data_being_used_in_the_chart_have_been_removed_for_the_simulation : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private DataRepository _observedData;
      private IndividualSimulation _simulation;
      private List<DataRepository> _dataRepositoriesRemoved;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _observedData = new DataRepository("OBS");
         _chart = new SimulationTimeProfileChart();
         _chart.AddObservedData(_observedData);
         sut.InitializeAnalysis(_chart);
         _simulation.DataRepository = new DataRepository();
         sut.UpdateAnalysisBasedOn(_simulation);

         _dataRepositoriesRemoved = new List<DataRepository>();
         A.CallTo(() => _chartEditorPresenter.RemoveDataRepositories(A<IEnumerable<DataRepository>>._))
            .Invokes(x => _dataRepositoriesRemoved.AddRange(x.GetArgument<IEnumerable<DataRepository>>(0)));
      }

      protected override void Because()
      {
         sut.Handle(new ObservedDataRemovedFromAnalysableEvent(_simulation, _observedData));
      }

      [Observation]
      public void should_remove_the_data_from_the_underlying_presenter()
      {
         _dataRepositoriesRemoved.ShouldContain(_observedData);
      }
   }

   public class When_notified_that_observed_data_being_used_in_the_chart_have_been_removed_for_another_simulation : concern_for_SimulationTimeProfileChartPresenter
   {
      private SimulationTimeProfileChart _chart;
      private DataRepository _observedData;
      private IndividualSimulation _simulation;
      private List<DataRepository> _dataRepositoriesRemoved;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _observedData = A.Fake<DataRepository>();
         _chart = new SimulationTimeProfileChart();
         _chart.AddObservedData(_observedData);
         sut.InitializeAnalysis(_chart);
         sut.UpdateAnalysisBasedOn(_simulation);


         _dataRepositoriesRemoved = new List<DataRepository>();
         A.CallTo(() => _chartEditorPresenter.RemoveDataRepositories(A<IEnumerable<DataRepository>>._))
            .Invokes(x => _dataRepositoriesRemoved.AddRange(x.GetArgument<IEnumerable<DataRepository>>(0)));
      }

      protected override void Because()
      {
         sut.Handle(new ObservedDataRemovedFromAnalysableEvent(A.Fake<Simulation>(), _observedData));
      }

      [Observation]
      public void should_not_remove_the_data_from_the_underlying_chart()
      {
         _chart.UsesObservedData(_observedData).ShouldBeTrue();
      }

      [Observation]
      public void should_not_remove_the_data_from_the_underlying_presenter()
      {
         _dataRepositoriesRemoved.ShouldNotContain(_observedData);
      }
   }
}