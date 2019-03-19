using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core;
using OSPSuite.Core.Domain;


namespace PKSim.Core
{
   public abstract class concern_for_ApplicationDiscriminator : ContextSpecification<IApplicationDiscriminator>
   {
      private IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _buildingBlockRepository= A.Fake<IBuildingBlockRepository>();
         sut = new ApplicationDiscriminator(_buildingBlockRepository);
      }
   }

   public class When_retrieving_the_discriminator_for_a_building_block : concern_for_ApplicationDiscriminator
   {
      [Observation]
      public void should_reutrn_the_building_block_type_as_string()
      {
         sut.DiscriminatorFor(new IndividualSimulation()).ShouldBeEqualTo(PKSimBuildingBlockType.Simulation.ToString());
         sut.DiscriminatorFor(new PopulationSimulation()).ShouldBeEqualTo(PKSimBuildingBlockType.Simulation.ToString());
         sut.DiscriminatorFor(new Compound()).ShouldBeEqualTo(PKSimBuildingBlockType.Compound.ToString());
         sut.DiscriminatorFor(new Event()).ShouldBeEqualTo(PKSimBuildingBlockType.Event.ToString());
         sut.DiscriminatorFor(new SimpleProtocol()).ShouldBeEqualTo(PKSimBuildingBlockType.Protocol.ToString());
         sut.DiscriminatorFor(new AdvancedProtocol()).ShouldBeEqualTo(PKSimBuildingBlockType.Protocol.ToString());
         sut.DiscriminatorFor(new Formulation()).ShouldBeEqualTo(PKSimBuildingBlockType.Formulation.ToString());
      }
   }

   public class When_retrieving_the_discriminator_for_an_object_that_is_not_a_building_block : concern_for_ApplicationDiscriminator
   {
      [Observation]
      public void should_return_the_type_as_string()
      {
         sut.DiscriminatorFor(new Parameter()).ShouldBeEqualTo(typeof(Parameter).Name);

      }
   }
}	