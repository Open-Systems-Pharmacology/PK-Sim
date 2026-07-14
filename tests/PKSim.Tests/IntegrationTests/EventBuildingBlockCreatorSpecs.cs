using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_EventBuildingBlockCreator : ContextForSimulationIntegration<IEventBuildingBlockCreator, Simulation>
   {
      protected Individual _individual;
      protected EventGroupBuildingBlock _result;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      protected IReadOnlyList<string> AdministeredMoleculesIn(string eventGroupName)
      {
         return _result.FindByName(eventGroupName).GetAllChildren<ApplicationBuilder>().Select(x => x.MoleculeName).ToList();
      }
   }

   public class When_creating_the_event_building_block_for_a_single_administered_compound : concern_for_EventBuildingBlockCreator
   {
      private Compound _compound;
      private Protocol _protocol;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound().WithName("C1");
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol);
      }

      [Observation]
      public void should_name_the_event_group_after_the_protocol()
      {
         _result.Select(x => x.Name).ShouldOnlyContain(_protocol.Name);
      }

      [Observation]
      public void should_administer_the_compound_in_the_event_group()
      {
         AdministeredMoleculesIn(_protocol.Name).ShouldOnlyContain(_compound.Name);
      }
   }

   public class When_creating_the_event_building_block_for_two_compounds_with_distinct_protocols : concern_for_EventBuildingBlockCreator
   {
      private Compound _compound1;
      private Compound _compound2;
      private Protocol _protocol1;
      private Protocol _protocol2;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound1 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C1");
         _compound2 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C2");
         _protocol1 = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("P1");
         _protocol2 = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("P2");
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] { _compound1, _compound2 }, new[] { _protocol1, _protocol2 });
      }

      [Observation]
      public void should_name_each_event_group_after_its_own_protocol()
      {
         _result.Select(x => x.Name).ShouldOnlyContain(_protocol1.Name, _protocol2.Name);
      }
   }

   public class When_creating_the_event_building_block_for_two_compounds_sharing_a_protocol : concern_for_EventBuildingBlockCreator
   {
      private Compound _compound1;
      private Compound _compound2;
      private Protocol _sharedProtocol;
      private string _eventGroupName1;
      private string _eventGroupName2;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound1 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C1");
         _compound2 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C2");
         _sharedProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _eventGroupName1 = Constants.CompositeNameFor(_sharedProtocol.Name, _compound1.Name);
         _eventGroupName2 = Constants.CompositeNameFor(_sharedProtocol.Name, _compound2.Name);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] { _compound1, _compound2 }, new[] { _sharedProtocol, _sharedProtocol });
      }

      [Observation]
      public void should_create_one_event_group_for_each_administered_compound()
      {
         _result.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_make_the_event_group_names_unique_by_combining_the_protocol_and_the_compound_name()
      {
         _result.Select(x => x.Name).ShouldOnlyContain(_eventGroupName1, _eventGroupName2);
      }

      [Observation]
      public void each_event_group_should_administer_only_its_own_compound()
      {
         AdministeredMoleculesIn(_eventGroupName1).ShouldOnlyContain(_compound1.Name);
         AdministeredMoleculesIn(_eventGroupName2).ShouldOnlyContain(_compound2.Name);
      }
   }

   //Protocol "A" is shared by C1 and C2 (event groups "A-C1" and "A-C2"), while a different single-use protocol
   //is literally named "A-C1". Its bare name would collide with the composite, so it is combined with its compound too.
   public class When_creating_the_event_building_block_for_a_protocol_named_like_the_composite_of_a_shared_protocol : concern_for_EventBuildingBlockCreator
   {
      private Compound _compound1;
      private Compound _compound2;
      private Compound _compound3;
      private Protocol _sharedProtocol;
      private Protocol _collidingProtocol;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound1 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C1");
         _compound2 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C2");
         _compound3 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C3");
         _sharedProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("A");
         _collidingProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName(Constants.CompositeNameFor("A", "C1"));
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual,
            new[] { _compound1, _compound2, _compound3 },
            new[] { _sharedProtocol, _sharedProtocol, _collidingProtocol });
      }

      [Observation]
      public void should_disambiguate_the_colliding_bare_name_by_combining_it_with_its_compound()
      {
         _result.Select(x => x.Name).ShouldOnlyContain(
            Constants.CompositeNameFor("A", "C1"),
            Constants.CompositeNameFor("A", "C2"),
            Constants.CompositeNameFor(_collidingProtocol.Name, "C3"));
      }
   }

   //Two different shared protocols produce the same composite name (protocol "A" + compound "B-C" and
   //protocol "A-B" + compound "C" both yield "A-B-C"), so the residual collision is resolved with a suffix.
   public class When_creating_the_event_building_block_for_compounds_whose_composite_names_collide : concern_for_EventBuildingBlockCreator
   {
      private Protocol _protocolA;
      private Protocol _protocolAB;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var compoundBC = DomainFactoryForSpecs.CreateStandardCompound().WithName("B-C");
         var compoundX = DomainFactoryForSpecs.CreateStandardCompound().WithName("X");
         var compoundC = DomainFactoryForSpecs.CreateStandardCompound().WithName("C");
         var compoundY = DomainFactoryForSpecs.CreateStandardCompound().WithName("Y");
         _protocolA = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("A");
         _protocolAB = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("A-B");
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual,
            new[] { compoundBC, compoundX, compoundC, compoundY },
            new[] { _protocolA, _protocolA, _protocolAB, _protocolAB });
      }

      [Observation]
      public void should_resolve_the_residual_composite_collision_with_a_numeric_suffix()
      {
         var duplicatedComposite = Constants.CompositeNameFor("A", "B-C"); //== CompositeNameFor("A-B", "C")
         _result.Select(x => x.Name).ShouldOnlyContain(
            duplicatedComposite,
            $"{duplicatedComposite}_2",
            Constants.CompositeNameFor("A", "X"),
            Constants.CompositeNameFor("A-B", "Y"));
      }
   }
}