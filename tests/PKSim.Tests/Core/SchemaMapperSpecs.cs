using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaMapper : ContextSpecificationAsync<SchemaMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected Schema _schema;
      protected SchemaItem _schemaItem;
      protected Snapshots.Schema _snapshot;
      protected IParameter _parameter;
      protected SchemaItemMapper _schemaItemMapper;
      protected ISchemaFactory _schemaFactory;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _schemaItemMapper = A.Fake<SchemaItemMapper>();
         _schemaFactory = A.Fake<ISchemaFactory>();

         sut = new SchemaMapper(_parameterMapper, _schemaItemMapper, _schemaFactory);

         _schemaItem = new SchemaItem().WithName("Item1");
         _schema = new Schema
         {
            Description = "Hello",
            Name = "Tralala"
         };
         _schema.AddSchemaItem(_schemaItem);

         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("Param1");
         _schema.Add(_parameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter)).Returns(new Snapshots.Parameter().WithName(_parameter.Name));

         A.CallTo(() => _schemaItemMapper.MapToSnapshot(_schemaItem)).Returns(new Snapshots.SchemaItem().WithName(_schemaItem.Name));

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_model_schema_to_a_snapshot_schema : concern_for_SchemaMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_schema);
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
         _snapshot.Parameters.ExistsByName(_parameter.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_save_all_schema_items()
      {
         _snapshot.SchemaItems.ExistsByName(_schemaItem.Name).ShouldBeTrue();
      }
   }

   public class When_mapping_a_valid_schema_snapshot_to_a_schema : concern_for_SchemaMapper
   {
      private Schema _newSchema;
      private SchemaItem _newSchemaItem;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_schema);
         A.CallTo(() => _schemaFactory.Create(null)).Returns(_schema);

         _snapshot.Name = "New SChema";
         _snapshot.Description = "The description that will be deserialized";

         _newSchemaItem = new SchemaItem().WithName("I am a new schema");
         A.CallTo(() => _schemaItemMapper.MapToModel(_snapshot.SchemaItems.FindByName(_schemaItem.Name))).Returns(_newSchemaItem);
      }

      protected override async Task Because()
      {
         _newSchema = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_have_created_a_schema_with_the_expected_properties()
      {
         _newSchema.Name.ShouldBeEqualTo(_snapshot.Name);
         _newSchema.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newSchema, PKSimConstants.ObjectTypes.Schema)).MustHaveHappened();
      }

      [Observation]
      public void should_have_added_schema_items_from_snapshot()
      {
         _schema.SchemaItems.ShouldContain(_newSchemaItem);
      }
   }
}