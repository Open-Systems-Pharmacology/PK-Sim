using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaMapper : ContextSpecification<SchemaMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected Schema _schema;
      protected SchemaItem _schemaItem1;
      protected SchemaItem _schemaItem2;
      protected Snapshots.Schema _snapshot;
      protected IParameter _parameter1;
      private SchemaItemMapper _schemaItemMapper;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _schemaItemMapper = A.Fake<SchemaItemMapper>();
         sut = new SchemaMapper(_parameterMapper, _schemaItemMapper);

         _schemaItem1 = new SchemaItem().WithName("Item1");
         _schemaItem2 = new SchemaItem().WithName("Item2");
         _schema = new Schema
         {
            Description = "Hello",
            Name = "Tralala"
         };
         _schema.AddSchemaItem(_schemaItem1);
         _schema.AddSchemaItem(_schemaItem2);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("Param1");
         _schema.Add(_parameter1);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Parameter().WithName(_parameter1.Name));

         A.CallTo(() => _schemaItemMapper.MapToSnapshot(_schemaItem1)).Returns(new Snapshots.SchemaItem().WithName(_schemaItem1.Name));
      }
   }

   public class When_mapping_a_model_schema_to_a_snapshot_schema : concern_for_SchemaMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_schema);
      }

      [Observation]
      public void should_save_the_schema_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_schema.Name);
         _snapshot.Description.ShouldBeEqualTo(_schema.Description);
      }

      [Observation]
      public void should_save_all_schema_parameters()
      {
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_save_all_schema_items()
      {
         _snapshot.SchemaItems.ExistsByName(_schemaItem1.Name).ShouldBeTrue();
      }
   }
}