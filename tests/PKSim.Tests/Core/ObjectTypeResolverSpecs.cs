using PKSim.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ObjectTypeResolver : ContextSpecification<IObjectTypeResolver>
   {
      protected override void Context()
      {
         sut = new ObjectTypeResolver();
      }
   }

   public class When_resolving_the_type_for_an_entity : concern_for_ObjectTypeResolver
   {
      [Observation]
      public void should_return_the_predefined_type_constant_for_that_entity()
      {
         sut.TypeFor(new Individual()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Individual);
         sut.TypeFor(new PKSimProject()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Project);
         sut.TypeFor(new Compound()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Compound);
         sut.TypeFor(new IndividualEnzyme()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Enzyme);
         sut.TypeFor(new PKSimParameter()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Parameter);
         sut.TypeFor(new IndividualSimulation()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Simulation);
         sut.TypeFor(new PopulationSimulation()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Simulation);
         sut.TypeFor(new Organ()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.Organ);
      }

      [Observation]
      public void should_return_the_type_of_the_object_if_the_entity_type_was_not_defined()
      {
         sut.TypeFor(new Container()).ShouldBeEqualTo("Container");
      }
   }

   public class When_resolving_the_type_from_an_entity_generic_type : concern_for_ObjectTypeResolver
   {
      [Observation]
      public void should_return_the_predefined_type_constant_for_that_entity()
      {
         sut.TypeFor<Individual>().ShouldBeEqualTo(PKSimConstants.ObjectTypes.Individual);
      }
   }

   public class When_resolving_the_type_for_a_distributed_parameter : concern_for_ObjectTypeResolver
   {
      [Observation]
      public void should_return_the_predefined_type_constant_for_that_entity()
      {
         sut.TypeFor(new PKSimDistributedParameter()).ShouldBeEqualTo(PKSimConstants.ObjectTypes.DistributedParameter);
      }
   }
}