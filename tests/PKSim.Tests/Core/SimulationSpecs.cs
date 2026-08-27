using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_Simulation : ContextSpecification<Simulation>
   {
      protected override void Context()
      {
         sut = new IndividualSimulation {Properties = new SimulationProperties()};
      }
   }

   public class When_checking_if_a_simulation_uses_an_event_template_referenced_by_an_event_placeholder_mapping : concern_for_Simulation
   {
      protected override void Context()
      {
         base.Context();
         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping {EventKey = "EVENT_1", TemplateEventId = "templateEventId"});
         sut.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_return_true_for_the_mapped_event_template()
      {
         sut.UsesEventTemplate("templateEventId").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_another_event_template()
      {
         sut.UsesEventTemplate("anotherTemplateEventId").ShouldBeFalse();
      }
   }

   public class When_checking_if_a_simulation_uses_an_event_template_referenced_by_an_event_mapping_from_the_events_tab : concern_for_Simulation
   {
      protected override void Context()
      {
         base.Context();
         sut.EventProperties.AddEventMapping(new EventMapping {TemplateEventId = "templateEventId"});
      }

      [Observation]
      public void should_return_true_for_the_mapped_event_template()
      {
         sut.UsesEventTemplate("templateEventId").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_another_event_template()
      {
         sut.UsesEventTemplate("anotherTemplateEventId").ShouldBeFalse();
      }
   }

   public class When_checking_if_a_simulation_without_any_event_mapping_uses_an_event_template : concern_for_Simulation
   {
      protected override void Context()
      {
         base.Context();
         sut.Properties.AddCompoundProperties(new CompoundProperties());
      }

      [Observation]
      public void should_return_false()
      {
         sut.UsesEventTemplate("templateEventId").ShouldBeFalse();
      }
   }

   public class When_a_simulation_load_flags_it_as_loaded : concern_for_Simulation
   {
      private BuildingBlockObservingOwnerFlag _buildingBlock;

      //records the owner's flag at the moment the child is flagged, pinning that a load flags the
      //children before publishing the owner's flag
      private class BuildingBlockObservingOwnerFlag : Individual
      {
         private readonly Simulation _owner;
         public bool? OwnerFlagWhenFlagged { get; private set; }

         public BuildingBlockObservingOwnerFlag(Simulation owner)
         {
            _owner = owner;
         }

         public override bool IsLoaded
         {
            set
            {
               OwnerFlagWhenFlagged = _owner.IsLoaded;
               base.IsLoaded = value;
            }
         }
      }

      protected override void Context()
      {
         base.Context();
         _buildingBlock = new BuildingBlockObservingOwnerFlag(sut);
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("template", PKSimBuildingBlockType.Individual) {BuildingBlock = _buildingBlock});
      }

      protected override void Because()
      {
         sut.IsLoaded = true;
      }

      [Observation]
      public void should_flag_the_used_building_blocks_before_publishing_its_own_flag()
      {
         _buildingBlock.OwnerFlagWhenFlagged.ShouldBeEqualTo(false);
      }

      [Observation]
      public void should_flag_the_used_building_blocks()
      {
         _buildingBlock.IsLoaded.ShouldBeTrue();
      }
   }
}
