using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolProperties : ContextSpecification<ProtocolProperties>
   {
      protected override void Context()
      {
         sut = new ProtocolProperties();
      }
   }

   public class When_adding_event_placeholder_mappings : concern_for_ProtocolProperties
   {
      private EventPlaceholderMapping _mapping1;
      private EventPlaceholderMapping _mapping2;

      protected override void Context()
      {
         base.Context();
         _mapping1 = new EventPlaceholderMapping { EventKey = "EVENT_1", TemplateEventId = "id1" };
         _mapping2 = new EventPlaceholderMapping { EventKey = "EVENT_2", TemplateEventId = "id2" };
      }

      protected override void Because()
      {
         sut.AddEventPlaceholderMapping(_mapping1);
         sut.AddEventPlaceholderMapping(_mapping2);
      }

      [Observation]
      public void should_contain_all_added_mappings()
      {
         sut.EventPlaceholderMappings.Count.ShouldBeEqualTo(2);
         sut.EventPlaceholderMappings.ShouldContain(_mapping1, _mapping2);
      }
   }

   public class When_retrieving_an_event_mapping_by_key : concern_for_ProtocolProperties
   {
      private EventPlaceholderMapping _mapping1;
      private EventPlaceholderMapping _mapping2;

      protected override void Context()
      {
         base.Context();
         _mapping1 = new EventPlaceholderMapping { EventKey = "EVENT_1" };
         _mapping2 = new EventPlaceholderMapping { EventKey = "EVENT_2" };
         sut.AddEventPlaceholderMapping(_mapping1);
         sut.AddEventPlaceholderMapping(_mapping2);
      }

      [Observation]
      public void should_return_the_mapping_with_the_matching_key()
      {
         sut.EventMappingWith("EVENT_1").ShouldBeEqualTo(_mapping1);
         sut.EventMappingWith("EVENT_2").ShouldBeEqualTo(_mapping2);
      }

      [Observation]
      public void should_return_null_for_unknown_key()
      {
         sut.EventMappingWith("UNKNOWN").ShouldBeNull();
      }
   }

   public class When_clearing_event_placeholder_mappings : concern_for_ProtocolProperties
   {
      protected override void Context()
      {
         base.Context();
         sut.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1" });
         sut.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_2" });
      }

      protected override void Because()
      {
         sut.ClearEventPlaceholderMappings();
      }

      [Observation]
      public void should_remove_all_event_placeholder_mappings()
      {
         sut.EventPlaceholderMappings.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_cloning_protocol_properties_with_event_placeholder_mappings : concern_for_ProtocolProperties
   {
      private ProtocolProperties _clone;
      private Protocol _protocol;

      protected override void Context()
      {
         base.Context();
         _protocol = A.Fake<Protocol>();
         sut.Protocol = _protocol;
         sut.AddFormulationMapping(new FormulationMapping { FormulationKey = "FORM_1" });
         sut.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_1", TemplateEventId = "id1" });
         sut.AddEventPlaceholderMapping(new EventPlaceholderMapping { EventKey = "EVENT_2", TemplateEventId = "id2" });
      }

      protected override void Because()
      {
         _clone = sut.Clone();
      }

      [Observation]
      public void should_copy_event_placeholder_mappings()
      {
         _clone.EventPlaceholderMappings.Count.ShouldBeEqualTo(2);
         _clone.EventPlaceholderMappings[0].EventKey.ShouldBeEqualTo("EVENT_1");
         _clone.EventPlaceholderMappings[1].EventKey.ShouldBeEqualTo("EVENT_2");
      }

      [Observation]
      public void should_copy_formulation_mappings()
      {
         _clone.FormulationMappings.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_copy_protocol_reference()
      {
         _clone.Protocol.ShouldBeEqualTo(_protocol);
      }
   }
}
