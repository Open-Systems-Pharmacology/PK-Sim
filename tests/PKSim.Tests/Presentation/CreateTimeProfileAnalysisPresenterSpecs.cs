using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateTimeProfileAnalysisPresenter : ContextSpecification<CreateTimeProfileAnalysisPresenter>
   {
      protected ICreatePopulationAnalysisView _view;
      protected ISubPresenterItemManager<IPopulationAnalysisItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;
      protected IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      protected IPopulationAnalysisChartFactory _populationAnalysisChartFactory;
      protected ILazyLoadTask _lazyLoadTask;
      protected IPopulationAnalysisParameterSelectionPresenter _parameterSelectionPresenter;
      protected IPopulationAnalysisPKParameterSelectionPresenter _pkParameterSpecificationPresenter;
      protected ICommandCollector _commandCollector;
      private IPopulationAnalysisTask _populationAnalysisTask;
      protected IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      protected TimeProfileAnalysisChart _populationAnalysisChart;

      protected override void Context()
      {
         _view = A.Fake<ICreatePopulationAnalysisView>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _populationAnalysisTemplateTask = A.Fake<IPopulationAnalysisTemplateTask>();
         _populationAnalysisChartFactory = A.Fake<IPopulationAnalysisChartFactory>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();

         _subPresenterManager = SubPresenterHelper.Create<IPopulationAnalysisItemPresenter>();
         _parameterSelectionPresenter = _subPresenterManager.CreateFake(TimeProfileItems.ParameterSelection);
         _pkParameterSpecificationPresenter = _subPresenterManager.CreateFake(TimeProfileItems.PKParameterSpecification);
         _populationAnalysisFieldFactory= A.Fake<IPopulationAnalysisFieldFactory>();
         _populationAnalysisChart = new TimeProfileAnalysisChart {PopulationAnalysis = new PopulationStatisticalAnalysis()};
         A.CallTo(() => _populationAnalysisChartFactory.Create<PopulationStatisticalAnalysis, TimeProfileAnalysisChart>()).Returns(_populationAnalysisChart);
         sut = new CreateTimeProfileAnalysisPresenter(_view, _subPresenterManager, _dialogCreator, _populationAnalysisTemplateTask, _populationAnalysisChartFactory, _lazyLoadTask, _populationAnalysisTask, _populationAnalysisFieldFactory);
      }
   }

   public class When_starting_the_create_time_profile_analysis_presenter : concern_for_CreateTimeProfileAnalysisPresenter
   {
      [Observation]
      public void should_hide_the_scaling_columns_for_pk_parameters_and_population_parameters ()
      {
         _parameterSelectionPresenter.ScalingVisible.ShouldBeFalse();
         _pkParameterSpecificationPresenter.ScalingVisible.ShouldBeFalse();
      }
   }

   public class When_creating_an_analysis_for_a_simulation_population_comparison : concern_for_CreateTimeProfileAnalysisPresenter
   {
      private PopulationSimulationComparison _populationSimulationComparison;
      private PopulationAnalysisCovariateField _simulationNameField;

      protected override void Context()
      {
         base.Context();
         _populationSimulationComparison = new PopulationSimulationComparison();
         _simulationNameField=  new PopulationAnalysisCovariateField();
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(CoreConstants.Covariates.SIMULATION_NAME, _populationSimulationComparison)).Returns(_simulationNameField);
      }

      protected override void Because()
      {
         sut.Create(_populationSimulationComparison);
      }

      [Observation]
      public void should_add_the_simulation_name_as_default_field_and_use_it_for_grouping_by_color()
      {
         _populationAnalysisChart.PopulationAnalysis.Has(_simulationNameField).ShouldBeTrue();
         var pivotAnalysis = _populationAnalysisChart.PopulationAnalysis.DowncastTo<PopulationPivotAnalysis>();
         pivotAnalysis.ColorField.ShouldBeEqualTo(_simulationNameField);
      }
   }

   public class When_editing_an_analysis_for_a_simulation_population_comparison : concern_for_CreateTimeProfileAnalysisPresenter
   {
      private PopulationSimulationComparison _populationSimulationComparison;
      private PopulationAnalysisCovariateField _simulationNameField;

      protected override void Context()
      {
         base.Context();
         _populationSimulationComparison = new PopulationSimulationComparison();
         _simulationNameField = new PopulationAnalysisCovariateField {Name = CoreConstants.Covariates.SIMULATION_NAME};
         _populationAnalysisChart.PopulationAnalysis.Add(_simulationNameField);
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(CoreConstants.Covariates.SIMULATION_NAME, _populationSimulationComparison)).Returns(_simulationNameField);
      }

      protected override void Because()
      {
         sut.Edit(_populationSimulationComparison, _populationAnalysisChart);
      }

      [Observation]
      public void should_not_add_the_simulation_analysis_field_if_it_already_exists()
      {
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(CoreConstants.Covariates.SIMULATION_NAME, _populationSimulationComparison)).MustNotHaveHappened();
      }
   }

   public class When_creating_an_analysis_for_a_simulation_population : concern_for_CreateTimeProfileAnalysisPresenter
   {
      private PopulationSimulation _populationSimulation;
      private PopulationAnalysisCovariateField _simulationNameField;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = new PopulationSimulation();
         _simulationNameField = new PopulationAnalysisCovariateField();
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(CoreConstants.Covariates.SIMULATION_NAME, _populationSimulation)).Returns(_simulationNameField);
      }

      protected override void Because()
      {
         sut.Create(_populationSimulation);
      }

      [Observation]
      public void should_not_add_the_simulation_name_as_default_field()
      {
         _populationAnalysisChart.PopulationAnalysis.Has(_simulationNameField).ShouldBeFalse();
      }
   }
}