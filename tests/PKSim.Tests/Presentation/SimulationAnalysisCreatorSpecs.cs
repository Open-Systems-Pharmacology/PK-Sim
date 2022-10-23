using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using SimulationAnalysisCreator = PKSim.Core.Services.SimulationAnalysisCreator;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationAnalysisCreator : ContextSpecification<SimulationAnalysisCreator>
   {
      protected IPopulationSimulationAnalysisStarter _populationSimulationAnalysisStarter;
      protected IExecutionContext _executionContext;
      protected IContainerTask _containerTask;
      protected IPKSimChartFactory _chartFactory;
      protected ICoreUserSettings _userSettings;
      protected ICloner _cloner;

      protected override void Context()
      {
         _populationSimulationAnalysisStarter = A.Fake<IPopulationSimulationAnalysisStarter>();
         _executionContext = A.Fake<IExecutionContext>();
         _containerTask = A.Fake<IContainerTask>();
         _chartFactory = A.Fake<IPKSimChartFactory>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _cloner = A.Fake<ICloner>();

         sut = new SimulationAnalysisCreator(_populationSimulationAnalysisStarter, _executionContext,
            _containerTask, _chartFactory, _userSettings, _cloner);
      }
   }

   public class When_creating_an_analysis_for_a_given_simulation : concern_for_SimulationAnalysisCreator
   {
      private IndividualSimulation _simulation;
      private SimulationTimeProfileChart _chart;
      private ISimulationAnalysis _chart1;
      private ISimulationAnalysis _chart2;
      private SimulationAnalysisCreatedEvent _plotEvent;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();
         _chart = new SimulationTimeProfileChart();
         A.CallTo(() => _chartFactory.CreateChartFor<SimulationTimeProfileChart>(_simulation)).Returns(_chart);
         _chart1 = new SimulationTimeProfileChart {Name = "Results 1"};
         _chart2 = new SimulationTimeProfileChart {Name = "My Results"};
         _simulation.AddAnalysis(_chart1);
         _simulation.AddAnalysis(_chart2);
         A.CallTo(_containerTask).WithReturnType<string>().Returns("Results 3");
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationAnalysisCreatedEvent>.Ignored)).Invokes(
            x => _plotEvent = x.GetArgument<SimulationAnalysisCreatedEvent>(0));
      }

      protected override void Because()
      {
         sut.CreateAnalysisFor(_simulation);
      }

      [Observation]
      public void should_create_a_plot_for_the_the_simulation()
      {
         _simulation.Analyses.ShouldContain(_chart);
      }

      [Observation]
      public void should_have_created_a_new_name_for_the_chart()
      {
         _chart.Name.ShouldBeEqualTo("Results 3");
      }

      [Observation]
      public void should_notify_that_the_plot_was_created_for_the_given_simulation()
      {
         _plotEvent.Analysable.ShouldBeEqualTo(_simulation);
         _plotEvent.SimulationAnalysis.ShouldBeEqualTo(_chart);
      }
   }

   public class When_creating_a_chart_for_a_population_simulation : concern_for_SimulationAnalysisCreator
   {
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _userSettings.DefaultPopulationAnalysis = PopulationAnalysisType.Scatter;
      }

      protected override void Because()
      {
         sut.CreatePopulationAnalysisFor(_populationDataCollector);
      }

      [Observation]
      public void should_start_the_analyses_selection()
      {
         A.CallTo(() => _populationSimulationAnalysisStarter.CreateAnalysisForPopulationSimulation(_populationDataCollector, _userSettings.DefaultPopulationAnalysis)).MustHaveHappened();
      }
   }

   public class When_creating_a_simulation_analysis_based_on_another_simulation_analysis : concern_for_SimulationAnalysisCreator
   {
      private ISimulationAnalysis _cloneAnalysis;
      private ISimulationAnalysis _sourceAnalysis;

      protected override void Context()
      {
         base.Context();
         _sourceAnalysis = A.Fake<ISimulationAnalysis>();
         _cloneAnalysis = A.Fake<ISimulationAnalysis>();
         A.CallTo(() => _cloner.CloneObject(_sourceAnalysis)).Returns(_cloneAnalysis);
      }

      [Observation]
      public void should_simply_returna_clone_of_the_source_analysis()
      {
         sut.CreateAnalysisBasedOn(_sourceAnalysis).ShouldBeEqualTo(_cloneAnalysis);
      }
   }
}