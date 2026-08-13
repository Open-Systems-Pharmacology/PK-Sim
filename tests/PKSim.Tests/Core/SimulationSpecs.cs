using OSPSuite.BDDHelper;
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
}
