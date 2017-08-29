using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_EntitiesInContainerRetriever : ContextSpecification<IEntitiesInContainerRetriever>
   {
      private IEntityPathResolver _entityPathResolver;
      protected IContainerTask _containerTask;

      protected override void Context()
      {
         _entityPathResolver = new EntityPathResolverForSpecs();
         _containerTask = A.Fake<IContainerTask>();
         sut = new EntitiesInContainerRetriever(_entityPathResolver, _containerTask);
      }
   }

   public class When_resolving_the_parameters_defined_in_a_population_simulation : concern_for_EntitiesInContainerRetriever
   {
      private PopulationSimulation _populationSimulation;
      private IParameter _p1;
      private IParameter _p2;
      private PathCache<IParameter> _pathCache;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _p1 = new PKSimParameter {Name = "P1"};
         _p2 = new PKSimParameter {Name = "P2"};
         _pathCache = new PathCacheForSpecs<IParameter> {_p1, _p2};
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_populationSimulation.Model.Root, A<Func<IParameter, bool>>._)).Returns(_pathCache);
      }

      [Observation]
      public void should_return_the_parameters_defined_in_the_model()
      {
         sut.ParametersFrom(_populationSimulation).ShouldOnlyContain(_p1, _p2);
      }

      [Observation]
      public void should_return_the_parameters_defined_in_the_model_when_called_as_a_data_collector()
      {
         sut.ParametersFrom((IPopulationDataCollector) _populationSimulation).ShouldOnlyContain(_p1, _p2);
      }
   }

   public class When_resolving_the_parameters_defined_in_a_population_simulation_comparison : concern_for_EntitiesInContainerRetriever
   {
      private PopulationSimulation _populationSimulation1;
      private readonly IParameter _p1 = new PKSimParameter {Name = "P1"};
      private readonly IParameter _p2 = new PKSimParameter {Name = "P2"};
      private readonly IParameter _p3 = new PKSimParameter {Name = "P3"};
      private readonly IParameter _p4 = new PKSimParameter {Name = "P4"};
      private PopulationSimulationComparison _comparison;
      private PopulationSimulation _populationSimulation2;

      protected override void Context()
      {
         base.Context();
         _comparison = A.Fake<PopulationSimulationComparison>();
         _populationSimulation1 = A.Fake<PopulationSimulation>();
         _populationSimulation2 = A.Fake<PopulationSimulation>();
         A.CallTo(() => _comparison.AllSimulations).Returns(new[] {_populationSimulation1, _populationSimulation2});
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_populationSimulation1.Model.Root, A<Func<IParameter, bool>>._)).Returns(new PathCacheForSpecs<IParameter> {_p1, _p2, _p4});
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_populationSimulation2.Model.Root, A<Func<IParameter, bool>>._)).Returns(new PathCacheForSpecs<IParameter> {_p2, _p3, _p4});
      }

      [Observation]
      public void should_return_the_intersection_of_all_parameters_defined_in_the_populations()
      {
         sut.ParametersFrom(_comparison).ShouldOnlyContain(_p2, _p4);
      }
   }

   public class When_retrieving_the_outputs_defined_in_a_simulation : concern_for_EntitiesInContainerRetriever
   {
      private Simulation _simulation;
      private readonly IQuantity _q1 = new Observer {Name = "O1"};
      private readonly IQuantity _q2 = new Observer {Name = "O2"};
      private readonly IQuantity _q3 = new Observer {Name = "O3"};

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.OutputSelections).Returns(new OutputSelections());
         var q1Selection = new QuantitySelection(_q1.Name, _q1.QuantityType);
         var q2Selection = new QuantitySelection(_q2.Name, _q2.QuantityType);
         _simulation.OutputSelections.AddOutput(q1Selection);
         _simulation.OutputSelections.AddOutput(q2Selection);
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_simulation.Model.Root, A<Func<IQuantity, bool>>._)).Returns(new PathCacheForSpecs<IQuantity> {_q1, _q2, _q3});
      }

      [Observation]
      public void should_returned_all_quantities_that_where_selected_in_the_simulation_settings()
      {
         sut.OutputsFrom(_simulation).ShouldOnlyContain(_q1, _q2);
      }
   }
}