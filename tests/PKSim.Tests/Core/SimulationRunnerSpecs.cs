using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationRunner : ContextSpecification<ISimulationRunner>
   {
      protected ISimulationEngine<IndividualSimulation> _simulationEngine;
      protected ISimulationEngine<PopulationSimulation> _popSimulationEngine;
      protected ISimulationEngineFactory _simulationEngineFactory;
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;
      protected ILazyLoadTask _lazyLoadTask;
      protected IEntityValidationTask _entityTask;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      private ICloner _cloner;

      protected override void Context()
      {
         _simulationEngine = A.Fake<ISimulationEngine<IndividualSimulation>>();
         _popSimulationEngine = A.Fake<ISimulationEngine<PopulationSimulation>>();
         _simulationEngineFactory = A.Fake<ISimulationEngineFactory>();
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _entityTask = A.Fake<IEntityValidationTask>();
         _cloner= A.Fake<ICloner>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         A.CallTo(() => _simulationEngineFactory.Create<PopulationSimulation>()).Returns(_popSimulationEngine);
         A.CallTo(() => _simulationEngineFactory.Create<IndividualSimulation>()).Returns(_simulationEngine);

         sut = new SimulationRunner(_simulationEngineFactory, _simulationAnalysisCreator, _lazyLoadTask, _entityTask, _simulationSettingsRetriever,_cloner);
      }
   }

   public class When_the_simulation_runner_was_told_to_run_a_simulation : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_simulation)).Returns(true);
      }

      protected override void Because()
      {
         sut.RunSimulation(_simulation);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _lazyLoadTask.Load<Simulation>(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_activate_the_simulation_engine_to_run_with_the_active_simulation()
      {
         A.CallTo(() => _simulationEngine.RunAsync(_simulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_is_notified_that_a_simulation_with_results_but_not_plot_was_calculated : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulation.Analyses).Returns(new List<ISimulationAnalysis>());
         A.CallTo(() => _simulation.HasResults).Returns(true);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_create_a_chart_for_the_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.CreateAnalysisFor(_simulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_is_notified_that_a_simulation_without_results_and_plot_was_calculated : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulation.Analyses).Returns(new List<SimulationTimeProfileChart>());
         A.CallTo(() => _simulation.HasResults).Returns(false);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_not_create_chart_for_the_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.CreateAnalysisFor(_simulation)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_is_notified_that_a_simulation_with_results_and_plot_was_calculated : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulation.Analyses).Returns(new List<ISimulationAnalysis> { A.Fake<ISimulationAnalysis>() });
         A.CallTo(() => _simulation.HasResults).Returns(true);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_not_create_chart_for_the_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.CreateAnalysisFor(_simulation)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_runner_was_told_to_stop_the_acticve_run : concern_for_SimulationRunner
   {
      private Simulation _activeSimulation;

      protected override void Context()
      {
         base.Context();
         _activeSimulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_activeSimulation)).Returns(true);
         sut.RunSimulation(_activeSimulation);
      }

      protected override void Because()
      {
         sut.StopSimulation();
      }

      [Observation]
      public void should_stop_the_simulation_engine()
      {
         A.CallTo(() => _simulationEngine.Stop()).MustHaveHappened();
      }
   }

   public class When_the_simulation_runner_is_told_to_stop_a_simulation_that_was_never_started : concern_for_SimulationRunner
   {
      protected override void Because()
      {
         sut.StopSimulation();
      }

      [Observation]
      public void should_not_crash()
      {
      }
   }

   public class When_the_simulation_runner_is_told_to_run_an_invalid_simulation_that_was_not_accepted_by_the_user : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_simulation)).Returns(false);
      }

      protected override void Because()
      {
         sut.RunSimulation(_simulation);
      }

      [Observation]
      public void should_not_run_the_simulation()
      {
         A.CallTo(() => _simulationEngine.RunAsync(_simulation)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_runner_is_starting_a_simulation_run_for_a_given_population_simulation : concern_for_SimulationRunner
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _entityTask.Validate(_populationSimulation)).Returns(true);
      }

      protected override void Because()
      {
         sut.RunSimulation(_populationSimulation);
      }

      [Observation]
      public void should_ask_the_user_to_select_the_quantities_that_should_be_persisted_for_the_run()
      {
         A.CallTo(() => _simulationSettingsRetriever.SettingsFor(_populationSimulation)).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_population_simulation_run : concern_for_SimulationRunner
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _entityTask.Validate(_populationSimulation)).Returns(true);
         A.CallTo(() => _simulationSettingsRetriever.SettingsFor(_populationSimulation)).Returns(null);
      }

      protected override void Because()
      {
         sut.RunSimulation(_populationSimulation);
      }

      [Observation]
      public void should_not_run_the_population_simulation()
      {
         A.CallTo(() => _popSimulationEngine.RunAsync(_populationSimulation)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_update_the_population_settings_in_the_popoulation()
      {
         _populationSimulation.OutputSelections.ShouldNotBeNull();
      }
   }

   public class When_the_user_confirms_the_simulation_run : concern_for_SimulationRunner
   {
      private PopulationSimulation _populationSimulation;
      private OutputSelections _newPopulationSettings;

      protected override void Context()
      {
         base.Context();
         _newPopulationSettings = new OutputSelections();
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _entityTask.Validate(_populationSimulation)).Returns(true);
         A.CallTo(() => _simulationSettingsRetriever.SettingsFor(_populationSimulation)).Returns(_newPopulationSettings);
      }

      protected override void Because()
      {
         sut.RunSimulation(_populationSimulation);
      }

      [Observation]
      public void should_run_the_population_simulation()
      {
         A.CallTo(() => _popSimulationEngine.RunAsync(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_population_settings_in_the_popoulation()
      {
         _populationSimulation.OutputSelections.AllOutputs.ShouldOnlyContain(_newPopulationSettings.AllOutputs);
      }
   }
}