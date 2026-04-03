using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SimpleProtocol_event : ContextSpecification<SimpleProtocol>
   {
      protected override void Context()
      {
         sut = new SimpleProtocol();
         sut.ApplicationType = ApplicationTypes.IntravenousBolus;
         sut.DosingInterval = DosingIntervals.Single;
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.EVENT_OFFSET));
      }
   }

   public class When_a_simple_protocol_has_no_event_set : concern_for_SimpleProtocol_event
   {
      [Observation]
      public void has_event_should_be_false()
      {
         sut.HasEvent.ShouldBeFalse();
      }

      [Observation]
      public void used_event_keys_should_be_empty()
      {
         sut.UsedEventKeys.ShouldBeEmpty();
      }
   }

   public class When_a_simple_protocol_has_an_event_set : concern_for_SimpleProtocol_event
   {
      protected override void Context()
      {
         base.Context();
         sut.EventKey = "event-id-123";
      }

      [Observation]
      public void has_event_should_be_true()
      {
         sut.HasEvent.ShouldBeTrue();
      }

      [Observation]
      public void used_event_keys_should_contain_the_template_event_id()
      {
         sut.UsedEventKeys.ShouldContain("event-id-123");
      }

      [Observation]
      public void used_event_keys_should_have_exactly_one_entry()
      {
         sut.UsedEventKeys.Count().ShouldBeEqualTo(1);
      }
   }

   public class When_a_simple_protocol_has_an_event_offset_parameter : concern_for_SimpleProtocol_event
   {
      [Observation]
      public void event_offset_parameter_should_not_be_null()
      {
         sut.EventOffsetParameter.ShouldNotBeNull();
      }

      [Observation]
      public void event_offset_parameter_should_have_the_correct_name()
      {
         sut.EventOffsetParameter.Name.ShouldBeEqualTo(CoreConstants.Parameters.EVENT_OFFSET);
      }
   }

   public class When_updating_properties_from_a_source_simple_protocol_with_event : concern_for_SimpleProtocol_event
   {
      private SimpleProtocol _source;

      protected override void Context()
      {
         base.Context();
         _source = new SimpleProtocol();
         _source.Add(DomainHelperForSpecs.ConstantParameterWithValue(1440).WithName(Constants.Parameters.END_TIME));
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(1440).WithName(Constants.Parameters.END_TIME));
         _source.ApplicationType = ApplicationTypes.Oral;
         _source.DosingInterval = DosingIntervals.DI_24;
         _source.FormulationKey = "Formulation";
         _source.EventKey = "EVENT_1";
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_source, A.Fake<ICloneManager>());
      }

      [Observation]
      public void should_copy_the_event_key()
      {
         sut.EventKey.ShouldBeEqualTo("EVENT_1");
      }

      [Observation]
      public void should_copy_the_application_type()
      {
         sut.ApplicationType.ShouldBeEqualTo(ApplicationTypes.Oral);
      }
   }

   public class When_clearing_event_from_simple_protocol : concern_for_SimpleProtocol_event
   {
      protected override void Context()
      {
         base.Context();
         sut.EventKey = "some-event-id";
      }

      protected override void Because()
      {
         sut.EventKey = null;
      }

      [Observation]
      public void has_event_should_be_false()
      {
         sut.HasEvent.ShouldBeFalse();
      }

      [Observation]
      public void used_event_keys_should_be_empty()
      {
         sut.UsedEventKeys.ShouldBeEmpty();
      }
   }
}
