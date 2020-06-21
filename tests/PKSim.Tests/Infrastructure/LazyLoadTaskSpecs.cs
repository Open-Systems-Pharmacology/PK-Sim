using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_LazyLoadTask : ContextSpecification<ILazyLoadTask>
   {
      protected IPKSimBuildingBlock _objectToLoad;
      protected IRegistrationTask _registrationTask;
      protected IProgressManager _progressManager;
      protected IProgressUpdater _progressUpdater;
      protected IContentLoader _contentLoader;
      protected ISimulationResultsLoader _simulationResultsLoader;
      protected ISimulationComparisonContentLoader _simulationComparisonContentLoader;
      protected ISimulationChartsLoader _simulationChartsLoader;
      protected ISimulationAnalysesLoader _simulationAnalysesLoader;
      private IParameterIdentificationContentLoader _parameterIdentificationContentendLoader;
      private ISensitivityAnalysisContentLoader _sensitivityAnalysisContentLoader;

      protected override void Context()
      {
         _registrationTask = A.Fake<IRegistrationTask>();
         _progressManager = A.Fake<IProgressManager>();
         _progressUpdater = A.Fake<IProgressUpdater>();
         _contentLoader = A.Fake<IContentLoader>();
         _simulationChartsLoader = A.Fake<ISimulationChartsLoader>();
         _simulationResultsLoader = A.Fake<ISimulationResultsLoader>();
         _simulationComparisonContentLoader = A.Fake<ISimulationComparisonContentLoader>();
         _simulationAnalysesLoader = A.Fake<ISimulationAnalysesLoader>();
         _parameterIdentificationContentendLoader = A.Fake<IParameterIdentificationContentLoader>();
         _sensitivityAnalysisContentLoader = A.Fake<ISensitivityAnalysisContentLoader>();

         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);
         sut = new LazyLoadTask(_contentLoader, _simulationResultsLoader, _simulationChartsLoader,
            _simulationComparisonContentLoader, _simulationAnalysesLoader, _parameterIdentificationContentendLoader, _sensitivityAnalysisContentLoader,
            _registrationTask, _progressManager);
         _objectToLoad = A.Fake<IPKSimBuildingBlock>();
         _objectToLoad.Id = "objectId";
      }
   }

   public class When_loading_a_lazy_loadable_object_that_was_already_loaded : concern_for_LazyLoadTask
   {
      protected override void Context()
      {
         base.Context();
         _objectToLoad.IsLoaded = true;
      }

      protected override void Because()
      {
         sut.Load(_objectToLoad);
      }

      [Observation]
      public void should_do_nothing()
      {
         A.CallTo(() => _contentLoader.LoadContentFor(_objectToLoad)).MustNotHaveHappened();
      }
   }

   public class When_loading_a_loadable_object_that_was_not_already_loaded : concern_for_LazyLoadTask
   {
      private IndividualSimulation _simulation;
      private UsedBuildingBlock _usedBB1;
      private UsedBuildingBlock _usedBB2;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _usedBB1 = A.Fake<UsedBuildingBlock>();
         _usedBB2 = A.Fake<UsedBuildingBlock>();
         A.CallTo(() => _simulation.UsedBuildingBlocks).Returns(new[] {_usedBB1, _usedBB2});
         _objectToLoad = _simulation;
         _objectToLoad.IsLoaded = false;
      }

      protected override void Because()
      {
         sut.Load(_objectToLoad);
      }

      [Observation]
      public void should_register_the_object_in_the_factory()
      {
         A.CallTo(() => _registrationTask.Register(_objectToLoad)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_content_of_the_object()
      {
         A.CallTo(() => _contentLoader.LoadContentFor<IObjectBase>(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_results_for_a_simulation()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_simulation_charts()
      {
         A.CallTo(() => _simulationChartsLoader.LoadChartsFor(_simulation)).MustHaveHappened();
      }
   }

   public class When_loading_a_simulation_object_that_was_not_already_loaded : concern_for_LazyLoadTask
   {
      protected override void Context()
      {
         base.Context();
         _objectToLoad.IsLoaded = false;
      }

      protected override void Because()
      {
         sut.Load(_objectToLoad);
      }

      [Observation]
      public void should_load_the_content_into_the_given_object()
      {
         A.CallTo(() => _contentLoader.LoadContentFor<IObjectBase>(_objectToLoad)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_object_in_the_factory()
      {
         A.CallTo(() => _registrationTask.Register(_objectToLoad)).MustHaveHappened();
      }
   }

   public class When_loading_an_individual_simulation : concern_for_LazyLoadTask
   {
      private IndividualSimulation _individualSimulation;

      protected override void Context()
      {
         base.Context();
         _individualSimulation = A.Fake<IndividualSimulation>();
      }

      protected override void Because()
      {
         sut.Load(_individualSimulation);
      }

      [Observation]
      public void should_also_load_the_simulation_results()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_individualSimulation)).MustHaveHappened();
      }
   }

   public class When_loading_a_population_simulation : concern_for_LazyLoadTask
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
      }

      protected override void Because()
      {
         sut.Load(_populationSimulation);
      }

      [Observation]
      public void should_not_load_the_simulation_results()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_populationSimulation)).MustNotHaveHappened();
      }

      [Observation]
      public void should_load_the_simulation_analyses()
      {
         A.CallTo(() => _simulationAnalysesLoader.LoadAnalysesFor(_populationSimulation)).MustHaveHappened();
      }
   }

   public class When_loading_the_simulation_results_for_a_simulation_that_already_has_results : concern_for_LazyLoadTask
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.HasResults).Returns(true);
      }

      protected override void Because()
      {
         sut.LoadResults(_simulation);
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_simulation)).MustNotHaveHappened();
      }
   }

   public class When_loading_the_simulation_results_for_a_simulation_that_has_no_results : concern_for_LazyLoadTask
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.HasResults).Returns(false);
      }

      protected override void Because()
      {
         sut.LoadResults(_simulation);
      }

      [Observation]
      public void should_load_the_results()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_simulation)).MustHaveHappened();
      }
   }

   public class When_loading_the_simulation_results_for_a_population_simulation_comparison : concern_for_LazyLoadTask
   {
      private PopulationSimulationComparison _comparison;
      private PopulationSimulation _sim1;
      private PopulationSimulation _sim2;

      protected override void Context()
      {
         base.Context();
         _comparison = new PopulationSimulationComparison();
         _sim1 = A.Fake<PopulationSimulation>().WithId("Sim1");
         _sim2 = A.Fake<PopulationSimulation>().WithId("Sim2");
         _comparison.AddSimulation(_sim1);
         _comparison.AddSimulation(_sim2);
      }

      protected override void Because()
      {
         sut.LoadResults(_comparison);
      }

      [Observation]
      public void should_load_the_results_for_all_underlying_simulations()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_sim1)).MustHaveHappened();
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_sim2)).MustHaveHappened();
      }
   }

   public class When_loading_the_results_of_a_simulation : concern_for_LazyLoadTask
   {
      private Simulation _simulationToLoad;

      protected override void Context()
      {
         base.Context();
         _simulationToLoad = new IndividualSimulation();
      }

      protected override void Because()
      {
         sut.LoadResults(_simulationToLoad);
      }

      [Observation]
      public void should_load_the_simulation_first()
      {
         A.CallTo(() => _contentLoader.LoadContentFor<IObjectBase>(_simulationToLoad)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_results()
      {
         A.CallTo(() => _simulationResultsLoader.LoadResultsFor(_simulationToLoad)).MustHaveHappened();
      }
   }
}