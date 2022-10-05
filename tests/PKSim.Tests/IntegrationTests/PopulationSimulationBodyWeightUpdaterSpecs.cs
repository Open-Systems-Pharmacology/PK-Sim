using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using Protocol = PKSim.Core.Model.Protocol;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PopulationSimulationBodyWeightUpdater : ContextForIntegration<PopulationSimulationBodyWeightUpdater>
   {
      private IEntityPathResolver _entityPathResolver;
      private Compound _compound;
      private Protocol _protocol;
      protected PopulationSimulation _simulation;

      protected override void Context()
      {
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);
         var population = DomainFactoryForSpecs.CreateDefaultPopulation(individual);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(population, _compound, _protocol) as PopulationSimulation;

         _entityPathResolver = new EntityPathResolverForSpecs();

         sut = new PopulationSimulationBodyWeightUpdater(_entityPathResolver);
      }
   }

   public class when_updating_body_weight_parameter_in_population_simulation : concern_for_PopulationSimulationBodyWeightUpdater
   {
      protected override void Because()
      {
         sut.UpdateBodyWeightForIndividual(_simulation, 1);
      }

      [Observation]
      public void should_update_the_body_weight_of_the_simulation_to_the_specified_individual()
      {
         _simulation.BodyWeight.Value.ShouldBeEqualTo(_simulation.AllValuesFor("Organism|Weight")[1]);
      }
   }

   public class when_resetting_the_body_weight_parameter_in_population_simulation : concern_for_PopulationSimulationBodyWeightUpdater
   {
      private double _originalValue;

      protected override void Context()
      {
         base.Context();
         _originalValue = _simulation.BodyWeight.Value;

         sut.UpdateBodyWeightForIndividual(_simulation, 1);
      }

      protected override void Because()
      {
         
         sut.ResetBodyWeightParameter(_simulation);
      }

      [Observation]
      public void should_reset_to_the_original_value()
      {
         _simulation.BodyWeight.Value.ShouldBeEqualTo(_originalValue);
      }
   }
}
