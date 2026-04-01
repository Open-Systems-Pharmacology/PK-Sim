using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_Schema : ContextSpecification<Schema>
   {
      protected ICloneManager _cloneManager;

      protected override void Context()
      {
         _cloneManager = new DummyCloneManager();
         sut = new Schema();
         //2 hours delay
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(120).WithName(Constants.Parameters.START_TIME));
         //6hours between repetitions
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(360).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         //repeat the schema 4 times
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));


         var schemaItem1 = new SchemaItem().WithName("SchemaItem1");
         schemaItem1.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schemaItem1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.INPUT_DOSE));
         schemaItem1.ApplicationType = ApplicationTypes.Intravenous;

         var schemaItem2 = new SchemaItem().WithName("SchemaItem2");
         schemaItem2.Add(DomainHelperForSpecs.ConstantParameterWithValue(180).WithName(Constants.Parameters.START_TIME));
         schemaItem2.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameters.INPUT_DOSE));
         schemaItem2.ApplicationType = ApplicationTypes.Intravenous;


         sut.AddSchemaItem(schemaItem1);
         sut.AddSchemaItem(schemaItem2);
      }

      protected class DummyCloneManager : ICloneManager
      {
         public T Clone<T>(T objectToClone) where T : class, IUpdatable
         {
            var schemaItem = objectToClone as ISchemaItem;
            if (schemaItem == null) return default(T);

            var cloneSchemaItem = new SchemaItem().WithName(Guid.NewGuid().ToString());
            cloneSchemaItem.FormulationKey = schemaItem.FormulationKey;
            cloneSchemaItem.ApplicationType = schemaItem.ApplicationType;
            cloneSchemaItem.EventPlaceholder = schemaItem.EventPlaceholder;
            cloneSchemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(schemaItem.StartTime.Value).WithName(Constants.Parameters.START_TIME));

            if (schemaItem.Dose != null)
               cloneSchemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(schemaItem.Dose.Value).WithName(CoreConstants.Parameters.INPUT_DOSE));

            return cloneSchemaItem.DowncastTo<T>();
         }

         public DataRepository Clone(DataRepository dataRepository)
         {
            return dataRepository;
         }
      }
   }

   public class When_the_schema_is_expanding_its_schema_item : concern_for_Schema
   {
      private IEnumerable<ISchemaItem> _result;

      protected override void Because()
      {
         _result = sut.ExpandedSchemaItems(_cloneManager);
      }

      [Observation]
      public void the_returned_schema_item_should_have_the_accurate_start_time_and_application_type()
      {
         _result.ElementAt(0).StartTime.Value.ShouldBeEqualTo(120);
         _result.ElementAt(1).StartTime.Value.ShouldBeEqualTo(300);
         _result.ElementAt(2).StartTime.Value.ShouldBeEqualTo(480);
         _result.ElementAt(3).StartTime.Value.ShouldBeEqualTo(660);
         _result.ElementAt(4).StartTime.Value.ShouldBeEqualTo(840);
         _result.ElementAt(5).StartTime.Value.ShouldBeEqualTo(1020);
         _result.ElementAt(6).StartTime.Value.ShouldBeEqualTo(1200);
         _result.ElementAt(7).StartTime.Value.ShouldBeEqualTo(1380);
      }

      [Observation]
      public void the_created_application_should_have_the_accurate_drug_mass()
      {
         _result.ElementAt(0).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(1).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(2).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(3).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(4).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(5).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(6).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(7).Dose.Value.ShouldBeEqualTo(2);
      }
   }

   public class When_the_schema_is_expanding_with_mixed_administration_and_event_entries : concern_for_Schema
   {
      private List<SchemaItem> _result;

      protected override void Context()
      {
         base.Context();

         var eventItem = new SchemaItem().WithName("EventItem");
         eventItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(Constants.Parameters.START_TIME));
         eventItem.ApplicationType = ApplicationTypes.Event;
         eventItem.EventPlaceholder = "EVENT_1";

         sut.AddSchemaItem(eventItem);
      }

      protected override void Because()
      {
         _result = sut.ExpandedSchemaItems(_cloneManager).ToList();
      }

      [Observation]
      public void should_expand_event_entries_alongside_administration_entries()
      {
         // 3 items per repetition * 4 repetitions = 12 total
         _result.Count.ShouldBeEqualTo(12);
      }

      [Observation]
      public void should_assign_correct_start_times_to_event_entries()
      {
         // Event items are at index 2, 5, 8, 11 (third item in each repetition)
         // start_time = 30 + schema_start(120) + repetition * offset(360)
         _result[2].StartTime.Value.ShouldBeEqualTo(150);
         _result[5].StartTime.Value.ShouldBeEqualTo(510);
         _result[8].StartTime.Value.ShouldBeEqualTo(870);
         _result[11].StartTime.Value.ShouldBeEqualTo(1230);
      }

      [Observation]
      public void should_preserve_event_placeholder_on_expanded_items()
      {
         _result[2].EventPlaceholder.ShouldBeEqualTo("EVENT_1");
         _result[5].EventPlaceholder.ShouldBeEqualTo("EVENT_1");
         _result[8].EventPlaceholder.ShouldBeEqualTo("EVENT_1");
         _result[11].EventPlaceholder.ShouldBeEqualTo("EVENT_1");
      }

      [Observation]
      public void should_mark_expanded_event_entries_as_event_type()
      {
         _result[2].IsEvent.ShouldBeTrue();
         _result[5].IsEvent.ShouldBeTrue();
      }

      [Observation]
      public void should_not_have_dose_on_event_entries()
      {
         _result[2].Dose.ShouldBeNull();
         _result[5].Dose.ShouldBeNull();
      }
   }

   public class When_the_schema_has_multiple_entries_with_same_event_placeholder : concern_for_Schema
   {
      private List<SchemaItem> _result;

      protected override void Context()
      {
         _cloneManager = new DummyCloneManager();
         sut = new Schema();
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(60).WithName(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS));
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameters.NUMBER_OF_REPETITIONS));

         var event1 = new SchemaItem().WithName("Event1");
         event1.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         event1.ApplicationType = ApplicationTypes.Event;
         event1.EventPlaceholder = "EVENT_1";

         var event2 = new SchemaItem().WithName("Event2");
         event2.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(Constants.Parameters.START_TIME));
         event2.ApplicationType = ApplicationTypes.Event;
         event2.EventPlaceholder = "EVENT_1";

         sut.AddSchemaItem(event1);
         sut.AddSchemaItem(event2);
      }

      protected override void Because()
      {
         _result = sut.ExpandedSchemaItems(_cloneManager).ToList();
      }

      [Observation]
      public void should_expand_all_entries_with_same_placeholder()
      {
         // 2 items per repetition * 2 repetitions = 4 total
         _result.Count.ShouldBeEqualTo(4);
         _result.All(x => x.EventPlaceholder == "EVENT_1").ShouldBeTrue();
      }

      [Observation]
      public void should_assign_correct_start_times()
      {
         _result[0].StartTime.Value.ShouldBeEqualTo(0);
         _result[1].StartTime.Value.ShouldBeEqualTo(30);
         _result[2].StartTime.Value.ShouldBeEqualTo(60);
         _result[3].StartTime.Value.ShouldBeEqualTo(90);
      }
   }
}