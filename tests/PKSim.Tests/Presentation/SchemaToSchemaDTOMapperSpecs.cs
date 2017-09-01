using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_SchemaToSchemaDTOMapper : ContextSpecification<ISchemaToSchemaDTOMapper>
   {
      protected ISchemaItemToSchemaItemDTOMapper _schemaItemDTOMapper;
      protected IParameterToParameterDTOInContainerMapper<SchemaDTO> _parameterDTOMapper;

      protected override void Context()
      {
         _schemaItemDTOMapper = A.Fake<ISchemaItemToSchemaItemDTOMapper>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOInContainerMapper<SchemaDTO>>();
         sut = new SchemaToSchemaDTOMapper(_schemaItemDTOMapper,_parameterDTOMapper);
      }
   }

   public class When_mapping_a_schema_to_a_schema_DTO : concern_for_SchemaToSchemaDTOMapper
   {
      private Schema _schema;
      private SchemaDTO _dto;
      private SchemaItem _schemaItemB;
      private SchemaItem _schemaItemA;

      protected override void Context()
      {
         base.Context();
         _schema = new Schema();
         _schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameter.TIME_BETWEEN_REPETITIONS).WithDimension(DomainHelperForSpecs.TimeDimensionForSpecs()));
         _schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameter.NUMBER_OF_REPETITIONS));
         _schema.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.START_TIME).WithDimension(DomainHelperForSpecs.TimeDimensionForSpecs()));

         A.CallTo(_parameterDTOMapper).WithReturnType<IParameterDTO>()
            .ReturnsLazily(x => new ParameterDTO(x.Arguments[0].DowncastTo<IParameter>()));

         _schemaItemB = new SchemaItem().WithName("B");
         _schemaItemA = new SchemaItem().WithName("A");
         _schema.AddSchemaItem(_schemaItemB);
         _schema.AddSchemaItem(_schemaItemA);

         A.CallTo(_schemaItemDTOMapper).WithReturnType<SchemaItemDTO>()
            .ReturnsLazily(x => new SchemaItemDTO(x.Arguments[0].DowncastTo<SchemaItem>()));
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_schema);
      }

      [Observation]
      public void should_return_a_schemaDTO_item_containing_the_properties_of_the_schema()
      {
         _dto.Schema.ShouldBeEqualTo(_schema);
         _dto.TimeBetweenRepetitionsParameter.Parameter.ShouldBeEqualTo(_schema.TimeBetweenRepetitions);
         _dto.StartTimeParameter.Parameter.ShouldBeEqualTo(_schema.StartTime);
         _dto.NumberOfRepetitionsParameter.Parameter.ShouldBeEqualTo(_schema.NumberOfRepetitions);
      }

      [Observation]
      public void should_add_one_schema_item_dto_for_each_schema_item_defined_in_the_schema_and_order_them_by_name()
      {
         _dto.SchemaItems.Count.ShouldBeEqualTo(2);
         _dto.SchemaItems[0].SchemaItem.ShouldBeEqualTo(_schemaItemA);
         _dto.SchemaItems[1].SchemaItem.ShouldBeEqualTo(_schemaItemB);
      }
   }
}