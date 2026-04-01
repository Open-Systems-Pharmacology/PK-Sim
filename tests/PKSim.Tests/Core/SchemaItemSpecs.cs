using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaItem : ContextSpecification<SchemaItem>
   {
      protected override void Context()
      {
         sut = new SchemaItem();
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
      }
   }

   public class When_checking_event_schema_item_properties : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = "EVENT_1";
      }

      [Observation]
      public void should_be_identified_as_event()
      {
         sut.IsEvent.ShouldBeTrue();
      }

      [Observation]
      public void should_not_be_oral()
      {
         sut.IsOral.ShouldBeFalse();
      }

      [Observation]
      public void should_not_be_iv()
      {
         sut.IsIV.ShouldBeFalse();
      }

      [Observation]
      public void should_not_be_user_defined()
      {
         sut.IsUserDefined.ShouldBeFalse();
      }

      [Observation]
      public void should_not_need_formulation()
      {
         sut.NeedsFormulation.ShouldBeFalse();
      }

      [Observation]
      public void should_have_no_dose_parameter()
      {
         sut.Dose.ShouldBeNull();
      }

      [Observation]
      public void should_store_event_placeholder()
      {
         sut.EventKey.ShouldBeEqualTo("EVENT_1");
      }
   }

   public class When_creating_an_administration_schema_item : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Intravenous;
      }

      [Observation]
      public void should_not_be_identified_as_event()
      {
         sut.IsEvent.ShouldBeFalse();
      }
   }

   public class When_validating_an_event_schema_item_with_placeholder : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = "EVENT_1";
      }

      [Observation]
      public void should_be_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_validating_an_event_schema_item_without_placeholder : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = string.Empty;
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_an_event_schema_item_with_null_placeholder : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = null;
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_an_event_schema_item_without_formulation : concern_for_SchemaItem
   {
      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = "EVENT_1";
         sut.FormulationKey = string.Empty;
      }

      [Observation]
      public void should_be_valid_because_events_do_not_need_formulation()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_cloning_an_event_schema_item : concern_for_SchemaItem
   {
      private SchemaItem _clone;

      protected override void Context()
      {
         base.Context();
         sut.ApplicationType = ApplicationTypes.Event;
         sut.EventKey = "EVENT_1";
      }

      protected override void Because()
      {
         _clone = new SchemaItem();
         _clone.UpdatePropertiesFrom(sut, null);
      }

      [Observation]
      public void should_copy_the_event_placeholder()
      {
         _clone.EventKey.ShouldBeEqualTo("EVENT_1");
      }

      [Observation]
      public void should_copy_the_application_type()
      {
         _clone.ApplicationType.ShouldBeEqualTo(ApplicationTypes.Event);
      }
   }
}
