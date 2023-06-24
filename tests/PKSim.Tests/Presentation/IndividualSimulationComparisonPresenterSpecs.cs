using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSimulationComparisonPresenter : ContextSpecification<IIndividualSimulationComparisonPresenter>
   {
      protected IIndividualSimulationComparisonView _view;
      protected IChartEditorAndDisplayPresenter _chartPresenter;
      protected IIndividualPKAnalysisPresenter _pkAnalysisPresenter;
      protected IChartTask _chartTask;
      protected IObservedDataTask _observedDataTask;
      protected ILazyLoadTask _lazyLoadTask;
      protected IChartEditorLayoutTask _chartLayoutTask;
      protected IChartTemplatingTask _chartTemplatingTask;
      protected IDataColumnToPathElementsMapper _dataColumnToPathElementsMapper;
      protected IProjectRetriever _projectRetriever;
      protected IUserSettings _userSettings;
      private ChartPresenterContext _chartPresenterContext;
      private ICurveNamer _curveNamer;
      private IChartUpdater _chartUpdateTask;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<IIndividualSimulationComparisonView>();
         _chartPresenter = A.Fake<IChartEditorAndDisplayPresenter>();
         _pkAnalysisPresenter = A.Fake<IIndividualPKAnalysisPresenter>();
         _curveNamer = A.Fake<ICurveNamer>();
         _chartTask = A.Fake<IChartTask>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _chartLayoutTask = A.Fake<IChartEditorLayoutTask>();
         _chartTemplatingTask = A.Fake<IChartTemplatingTask>();
         _dataColumnToPathElementsMapper = A.Fake<IDataColumnToPathElementsMapper>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _chartPresenterContext = A.Fake<ChartPresenterContext>();
         _chartUpdateTask = A.Fake<IChartUpdater>();
         _userSettings = A.Fake<IUserSettings>();
         _dialogCreator = A.Fake<IDialogCreator>();
         A.CallTo(() => _chartPresenterContext.EditorAndDisplayPresenter).Returns(_chartPresenter);
         A.CallTo(() => _chartPresenterContext.CurveNamer).Returns(_curveNamer);
         A.CallTo(() => _chartPresenterContext.EditorLayoutTask).Returns(_chartLayoutTask);
         A.CallTo(() => _chartPresenterContext.TemplatingTask).Returns(_chartTemplatingTask);
         A.CallTo(() => _chartPresenterContext.ProjectRetriever).Returns(_projectRetriever);

         sut = new IndividualSimulationComparisonPresenter(_view, _chartPresenterContext, _pkAnalysisPresenter,
            _chartTask, _observedDataTask, _lazyLoadTask, _chartTemplatingTask, _chartUpdateTask, _userSettings, _dialogCreator);
      }
   }

   public class When_adding_a_simulation_to_the_summary_chart_containing_curves : concern_for_IndividualSimulationComparisonPresenter
   {
      private IDragEvent _dropEventArgs;
      private IndividualSimulation _simulation;
      private IndividualSimulationComparison _individualSimulationComparison;
      private IDimensionFactory _dimensionFactory;

      protected override void Context()
      {
         base.Context();
         _individualSimulationComparison = new IndividualSimulationComparison();
         _dimensionFactory = A.Fake<IDimensionFactory>();
         var dataRepository = DomainHelperForSpecs.ObservedData();
         var curve = new Curve();
         curve.SetxData(dataRepository.BaseGrid, _dimensionFactory);
         curve.SetyData(dataRepository.FirstDataColumn(), _dimensionFactory);
         _individualSimulationComparison.AddCurve(curve);
         sut.InitializeAnalysis(_individualSimulationComparison);
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulation.HasResults).Returns(true);
         var simulationNode = new SimulationNode(new ClassifiableSimulation {Subject = _simulation});
         _dropEventArgs = A.Fake<IDragEvent>();
         A.CallTo(() => _dropEventArgs.Data<IReadOnlyList<ITreeNode>>()).Returns(new List<SimulationNode> {simulationNode});
      }

      protected override void Because()
      {
         sut.DragDrop(this, _dropEventArgs);
      }

      [Observation]
      public void should_not_initiate_the_creation_from_template()
      {
         A.CallTo(() => _chartTemplatingTask.InitFromTemplate(
               A<CurveChart>._, A<IChartEditorAndDisplayPresenter>._, A<IReadOnlyCollection<DataColumn>>._,
               A<IReadOnlyCollection<IndividualSimulation>>._, A<Func<DataColumn, string>>._, null))
            .MustNotHaveHappened();
      }

      [Observation]
      public void should_simply_update_the_new_curve_defined_in_the_simulation()
      {
         A.CallTo(() => _chartTemplatingTask.UpdateDefaultSettings(_chartPresenter.EditorPresenter, A<IReadOnlyCollection<DataColumn>>._, A<IReadOnlyCollection<IndividualSimulation>>._, false, null)).MustHaveHappened();
      }
   }

   public class When_adding_a_simulation_to_the_summary_without_results : concern_for_IndividualSimulationComparisonPresenter
   {
      private IDragEvent _dropEventArgs;
      private IndividualSimulation _simulation;
      private IndividualSimulationComparison _individualSimulationComparison;

      protected override void Context()
      {
         base.Context();
         _individualSimulationComparison = new IndividualSimulationComparison();

         sut.InitializeAnalysis(_individualSimulationComparison);
         _simulation = A.Fake<IndividualSimulation>().WithName("test");
         A.CallTo(() => _simulation.HasResults).Returns(false);
         var simulationNode = new SimulationNode(new ClassifiableSimulation {Subject = _simulation});
         _dropEventArgs = A.Fake<IDragEvent>();
         A.CallTo(() => _dropEventArgs.Data<IReadOnlyList<ITreeNode>>()).Returns(new List<SimulationNode> {simulationNode});
      }

      protected override void Because()
      {
         sut.DragDrop(this, _dropEventArgs);
      }

      [Observation]
      public void should_show_a_warning_that_the_simulation_needs_to_be_run_first_to_be_visible()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(PKSimConstants.Error.SimulationHasNoResultsAndCannotBeUsedInComparison(_simulation.Name))).MustHaveHappened();
      }

      [Observation]
      public void should_not_try_to_initialize_the_curve_with_default_settings()
      {
         A.CallTo(_chartTemplatingTask).MustNotHaveHappened();
      }
   }

   public class When_editing_an_individual_simulation_comparison : concern_for_IndividualSimulationComparisonPresenter
   {
      private IndividualSimulationComparison _individualSimulationComparison;

      protected override void Context()
      {
         base.Context();
         _individualSimulationComparison = new IndividualSimulationComparison();
      }

      protected override void Because()
      {
         sut.Edit(_individualSimulationComparison);
      }

      [Observation]
      public void should_bind_the_chart_to_all_editor_even_when_no_simulation_is_used_in_the_comparison()
      {
         A.CallTo(() => _chartPresenter.DisplayPresenter.Edit(_individualSimulationComparison)).MustHaveHappened();
      }
   }


   public class When_editing_an_individual_simulation_comparison_without_any_predefined_curves_in_a_simulation : concern_for_IndividualSimulationComparisonPresenter
   {
      private IndividualSimulationComparison _individualSimulationComparison;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation= new IndividualSimulation
         {
            DataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("sim")
         };
         _individualSimulationComparison = new IndividualSimulationComparison();
         _individualSimulationComparison.AddSimulation(_simulation);
      }

      protected override void Because()
      {
         sut.Edit(_individualSimulationComparison);
      }

      [Observation]
      public void should_initialize_the_curves_in_the_chart()
      {
         A.CallTo(() => _chartTemplatingTask.UpdateDefaultSettings(_chartPresenter.EditorPresenter, A<IReadOnlyCollection<DataColumn>>._, A<IReadOnlyCollection<IndividualSimulation>>._, false, null)).MustHaveHappened();
      }
   }

   public class When_editing_an_individual_simulation_comparison_with_any_predefined_curves_in_a_simulation : concern_for_IndividualSimulationComparisonPresenter
   {
      private IndividualSimulationComparison _individualSimulationComparison;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation
         {
            DataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("sim")
         };
         _individualSimulationComparison = new IndividualSimulationComparison();
         _individualSimulationComparison.AddSimulation(_simulation);
         var dimensionFactory = A.Fake<IDimensionFactory>();
         var dataRepository = DomainHelperForSpecs.ObservedData();
         var curve = new Curve();
         curve.SetxData(dataRepository.BaseGrid, dimensionFactory);
         curve.SetyData(dataRepository.FirstDataColumn(), dimensionFactory);
         _individualSimulationComparison.AddCurve(curve);
      }

      protected override void Because()
      {
         sut.Edit(_individualSimulationComparison);
      }

      [Observation]
      public void should_initialize_the_curves_in_the_chart()
      {
         A.CallTo(() => _chartTemplatingTask.UpdateDefaultSettings(_chartPresenter.EditorPresenter, A<IReadOnlyCollection<DataColumn>>._, A<IReadOnlyCollection<IndividualSimulation>>._, false, null)).MustNotHaveHappened();
      }
   }

}