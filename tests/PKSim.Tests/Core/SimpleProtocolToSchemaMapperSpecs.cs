using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimpleProtocolToSchemaMapper : ContextSpecification<ISimpleProtocolToSchemaMapper>
   {
      private ISchemaFactory _schemaFactory;
      private ISchemaItemFactory _schemaItemFactory;
      private ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      private ISchemaItemRepository _schemaItemRepo;
      private ICloner _cloner;

      protected override void Context()
      {
         _schemaFactory = new SchemaFactoryForTest();
         _schemaItemFactory = new SchemaItemFactoryForTest();
         _schemaItemRepo = A.Fake<ISchemaItemRepository>();
         _cloner = A.Fake<ICloner>();
         _schemaItemParameterRetriever = new SchemaItemParameterRetriever(_schemaItemRepo, _cloner);
         sut = new SimpleProtocolToSchemaMapper(_schemaFactory, _schemaItemFactory, _schemaItemParameterRetriever);
      }

      internal class SchemaItemFactoryForTest : ISchemaItemFactory
      {
         public SchemaItem Create()
         {
            var schemaItem = new SchemaItem().WithName(Guid.NewGuid().ToString());
            schemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
            schemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.INPUT_DOSE));
            return schemaItem;
         }

         public SchemaItem Create(ApplicationType applicationType, IContainer container)
         {
            var schemaItem = Create();
            schemaItem.ApplicationType = applicationType;
            return schemaItem;
         }

         public SchemaItem CreateBasedOn(SchemaItem schemaItemToClone, IContainer container)
         {
            return Create(schemaItemToClone.ApplicationType, container);
         }
      }

      internal class SchemaFactoryForTest : ISchemaFactory
      {
         public Schema CreateWithDefaultItem(ApplicationType applicationType, IContainer container)
         {
            var schema = Create(container);
            schema.Add(new SchemaFactoryForTest().Create(container));
            return schema;
         }

         public Schema Create(IContainer container)
         {
            var schema = new Schema().WithName(Guid.NewGuid().ToString());
            schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
            schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.TIME_BETWEEN_REPETITIONS));
            schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.NUMBER_OF_REPETITIONS));
            return schema;
         }
      }
   }

   public class When_mapping_a_simple_protocol_with_a_single_dosing_application : concern_for_SimpleProtocolToSchemaMapper
   {
      private SimpleProtocol _simpleProtocol;
      private IEnumerable<Schema> _result;

      protected override void Context()
      {
         base.Context();
         _simpleProtocol = new SimpleProtocol
         {
            ApplicationType = ApplicationTypes.Intravenous,
            DosingInterval = DosingIntervals.Single
         };
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.END_TIME));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(3).WithName(Constants.Parameters.START_TIME));
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simpleProtocol);
      }

      [Observation]
      public void should_create_a_schema_with_only_one_repetition_and_one_schema_item()
      {
         _result.Count().ShouldBeEqualTo(1);
         var schema = _result.ElementAt(0);
         schema.NumberOfRepetitions.Value.ShouldBeEqualTo(1);
         schema.StartTime.Value.ShouldBeEqualTo(0);
         schema.TimeBetweenRepetitions.Value.ShouldBeEqualTo(0);
         schema.SchemaItems.Count().ShouldBeEqualTo(1);
         var schemaItem = schema.SchemaItems.ElementAt(0);
         schemaItem.ApplicationType.ShouldBeEqualTo(_simpleProtocol.ApplicationType);
         schemaItem.Dose.Value.ShouldBeEqualTo(_simpleProtocol.Dose.Value);
         schemaItem.StartTime.Value.ShouldBeEqualTo(_simpleProtocol.StartTime.Value);
      }
   }

   public class When_mapping_a_simple_protocol_with_a_bidaily_dosing_interval_matching_exatly_the_end_time : concern_for_SimpleProtocolToSchemaMapper
   {
      private SimpleProtocol _simpleProtocol;
      private IEnumerable<Schema> _result;

      protected override void Context()
      {
         base.Context();
         _simpleProtocol = new SimpleProtocol();
         _simpleProtocol.ApplicationType = ApplicationTypes.Intravenous;
         _simpleProtocol.DosingInterval = DosingIntervals.DI_12_12;
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(2880).WithName(Constants.Parameters.END_TIME));
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simpleProtocol);
      }

      [Observation]
      public void should_return_one_schema_for_which_the_number_of_repetition_is_set_to_match_the_protocol_length()
      {
         _result.Count().ShouldBeEqualTo(1);
         var schema = _result.ElementAt(0);
         //Number of repetition should be equal to 5 (0,12,24,36)
         schema.NumberOfRepetitions.Value.ShouldBeEqualTo(4);
         schema.StartTime.Value.ShouldBeEqualTo(0);
         schema.TimeBetweenRepetitions.Value.ShouldBeEqualTo(720); //12h
         schema.SchemaItems.Count().ShouldBeEqualTo(1);
         var schemaItem = schema.SchemaItems.ElementAt(0);
         schemaItem.ApplicationType.ShouldBeEqualTo(_simpleProtocol.ApplicationType);
         schemaItem.Dose.Value.ShouldBeEqualTo(_simpleProtocol.Dose.Value);
      }
   }

   public class When_mapping_a_simple_protocol_with_a_6_6_12_dosing_interval_which_does_not_match_exatly_the_end_time_but_contains_at_least_one_full_repetition : concern_for_SimpleProtocolToSchemaMapper
   {
      private SimpleProtocol _simpleProtocol;
      private IEnumerable<Schema> _result;

      protected override void Context()
      {
         base.Context();
         _simpleProtocol = new SimpleProtocol();
         _simpleProtocol.ApplicationType = ApplicationTypes.Intravenous;
         _simpleProtocol.DosingInterval = DosingIntervals.DI_6_6_12;
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(3600).WithName(Constants.Parameters.END_TIME)); //2.5 day
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simpleProtocol);
      }

      [Observation]
      public void should_return_two_schemas_for_the_repetition_part_and_the_singular_part()
      {
         _result.Count().ShouldBeEqualTo(2);
         var schema = _result.ElementAt(0);
         //Number of repetition should be equal to 5 (0,12,24,36)
         schema.NumberOfRepetitions.Value.ShouldBeEqualTo(2);
         schema.StartTime.Value.ShouldBeEqualTo(0);
         schema.TimeBetweenRepetitions.Value.ShouldBeEqualTo(1440); //24h
         schema.SchemaItems.Count().ShouldBeEqualTo(3);
         var schemaItem = schema.SchemaItems.ElementAt(0);
         schemaItem.ApplicationType.ShouldBeEqualTo(_simpleProtocol.ApplicationType);
         schemaItem.Dose.Value.ShouldBeEqualTo(_simpleProtocol.Dose.Value);

         var schemaSingular = _result.ElementAt(1);
         schemaSingular.NumberOfRepetitions.Value.ShouldBeEqualTo(1);
         schemaSingular.StartTime.Value.ShouldBeEqualTo(2880);
      }
   }
}