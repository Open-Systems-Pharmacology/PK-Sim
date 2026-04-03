using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_AdvancedProtocol : ContextSpecification<AdvancedProtocol>
   {
      protected override void Context()
      {
         sut = new AdvancedProtocol
         {
            TimeUnit = DomainHelperForSpecs.TimeDimensionForSpecs().DefaultUnit
         };
      }

      private int _schemaItemCounter;

      protected SchemaItem CreateSchemaItem(ApplicationType applicationType, string formulationKey = "", string eventKey = "")
      {
         _schemaItemCounter++;
         var schemaItem = new SchemaItem
         {
            ApplicationType = applicationType,
            FormulationKey = formulationKey,
            EventKey = eventKey
         }.WithName($"SchemaItem_{_schemaItemCounter}");
         schemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.INPUT_DOSE));
         return schemaItem;
      }
   }

   public class When_an_advanced_protocol_has_no_event_schema_items : concern_for_AdvancedProtocol
   {
      protected override void Context()
      {
         base.Context();
         var schema = new Schema().WithName("Schema1");
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.IntravenousBolus));
         sut.AddSchema(schema);
      }

      [Observation]
      public void used_event_keys_should_be_empty()
      {
         sut.UsedEventKeys.ShouldBeEmpty();
      }
   }

   public class When_an_advanced_protocol_has_event_schema_items_with_keys : concern_for_AdvancedProtocol
   {
      protected override void Context()
      {
         base.Context();
         var schema = new Schema().WithName("Schema1");
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: "EVENT_1"));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.IntravenousBolus));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: "EVENT_2"));
         sut.AddSchema(schema);
      }

      [Observation]
      public void used_event_keys_should_contain_the_event_keys()
      {
         sut.UsedEventKeys.ShouldContain("EVENT_1", "EVENT_2");
      }

      [Observation]
      public void used_event_keys_should_have_two_entries()
      {
         sut.UsedEventKeys.Count().ShouldBeEqualTo(2);
      }
   }

   public class When_an_advanced_protocol_has_duplicate_event_keys : concern_for_AdvancedProtocol
   {
      protected override void Context()
      {
         base.Context();
         var schema1 = new Schema().WithName("Schema1");
         schema1.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schema1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));
         schema1.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         schema1.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: "EVENT_1"));
         sut.AddSchema(schema1);

         var schema2 = new Schema().WithName("Schema2");
         schema2.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schema2.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));
         schema2.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         schema2.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: "EVENT_1"));
         sut.AddSchema(schema2);
      }

      [Observation]
      public void used_event_keys_should_return_distinct_keys()
      {
         sut.UsedEventKeys.Count().ShouldBeEqualTo(1);
      }
   }

   public class When_an_advanced_protocol_has_event_items_with_empty_keys : concern_for_AdvancedProtocol
   {
      protected override void Context()
      {
         base.Context();
         var schema = new Schema().WithName("Schema1");
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));
         schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(24).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: ""));
         schema.AddSchemaItem(CreateSchemaItem(ApplicationTypes.Event, eventKey: "EVENT_1"));
         sut.AddSchema(schema);
      }

      [Observation]
      public void should_exclude_empty_keys()
      {
         sut.UsedEventKeys.ShouldOnlyContain("EVENT_1");
      }
   }
}
