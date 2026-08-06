using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationAllEventsFromProtocolPlaceholderMappings : ContextSpecification<Simulation>
   {
      protected override void Context()
      {
         sut = new IndividualSimulation { Properties = new SimulationProperties() };
      }
   }

   public class When_retrieving_all_events_from_protocol_placeholder_mappings_with_no_mappings : concern_for_SimulationAllEventsFromProtocolPlaceholderMappings
   {
      protected override void Context()
      {
         base.Context();
         sut.Properties.AddCompoundProperties(new CompoundProperties());
      }

      [Observation]
      public void should_return_an_empty_list()
      {
         sut.AllEventsFromProtocolPlaceholderMappings().Count.ShouldBeEqualTo(0);
      }
   }

   public class When_retrieving_all_events_from_protocol_placeholder_mappings_with_mappings : concern_for_SimulationAllEventsFromProtocolPlaceholderMappings
   {
      private PKSimEvent _event1;
      private PKSimEvent _event2;

      protected override void Context()
      {
         base.Context();
         _event1 = new PKSimEvent().WithId("e1");
         _event2 = new PKSimEvent().WithId("e2");

         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", Event = _event1 });
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_2", Event = _event2 });
         sut.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_return_all_distinct_events()
      {
         var events = sut.AllEventsFromProtocolPlaceholderMappings();
         events.Count.ShouldBeEqualTo(2);
         events.ShouldContain(_event1, _event2);
      }
   }

   public class When_retrieving_events_with_duplicate_event_across_placeholders : concern_for_SimulationAllEventsFromProtocolPlaceholderMappings
   {
      private PKSimEvent _event1;

      protected override void Context()
      {
         base.Context();
         _event1 = new PKSimEvent().WithId("e1");

         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", Event = _event1 });
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_2", Event = _event1 });
         sut.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_return_distinct_events_only()
      {
         sut.AllEventsFromProtocolPlaceholderMappings().Count.ShouldBeEqualTo(1);
      }
   }

   public class When_retrieving_events_with_null_event_in_mapping : concern_for_SimulationAllEventsFromProtocolPlaceholderMappings
   {
      private PKSimEvent _event1;

      protected override void Context()
      {
         base.Context();
         _event1 = new PKSimEvent().WithId("e1");

         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", Event = _event1 });
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_2", Event = null });
         sut.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_filter_out_null_events()
      {
         var events = sut.AllEventsFromProtocolPlaceholderMappings();
         events.Count.ShouldBeEqualTo(1);
         events.First().ShouldBeEqualTo(_event1);
      }
   }

   public class When_retrieving_events_across_multiple_compounds : concern_for_SimulationAllEventsFromProtocolPlaceholderMappings
   {
      private PKSimEvent _event1;
      private PKSimEvent _event2;

      protected override void Context()
      {
         base.Context();
         _event1 = new PKSimEvent().WithId("e1");
         _event2 = new PKSimEvent().WithId("e2");

         var compoundProperties1 = new CompoundProperties();
         compoundProperties1.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", Event = _event1 });

         var compoundProperties2 = new CompoundProperties();
         compoundProperties2.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", Event = _event2 });

         sut.Properties.AddCompoundProperties(compoundProperties1);
         sut.Properties.AddCompoundProperties(compoundProperties2);
      }

      [Observation]
      public void should_gather_events_from_all_compounds()
      {
         var events = sut.AllEventsFromProtocolPlaceholderMappings();
         events.Count.ShouldBeEqualTo(2);
         events.ShouldContain(_event1, _event2);
      }
   }
}
