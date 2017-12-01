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
   public abstract class concern_for_SchemaItemMapper : ContextSpecificationAsync<SchemaItemMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected SchemaItem _schemaItem;
      protected Snapshots.SchemaItem _snapshot;
      protected IParameter _parameter;
      protected ISchemaItemFactory _schemaItemFactory;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _schemaItemFactory = A.Fake<ISchemaItemFactory>();

         sut = new SchemaItemMapper(_parameterMapper, _schemaItemFactory);

         _schemaItem = new SchemaItem
         {
            Name = "SchemaItem",
            Description = "The schema item",
            ApplicationType = ApplicationTypes.Intravenous,
            FormulationKey = "F1",
            TargetCompartment = "Cells",
            TargetOrgan = "Liver"
         };

         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("Param1");
         _schemaItem.Add(_parameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter)).Returns(new Snapshots.Parameter().WithName(_parameter.Name));

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_model_schema_item_to_a_snapshot_schema_item : concern_for_SchemaItemMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_schemaItem);
      }

      [Observation]
      public void should_save_the_schema_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_schemaItem.Name);
         _snapshot.Description.ShouldBeEqualTo(_schemaItem.Description);
         _snapshot.TargetOrgan.ShouldBeEqualTo(_schemaItem.TargetOrgan);
         _snapshot.TargetCompartment.ShouldBeEqualTo(_schemaItem.TargetCompartment);
         _snapshot.FormulationKey.ShouldBeEqualTo(_schemaItem.FormulationKey);
      }

      [Observation]
      public void should_save_all_schema_items_parameters()
      {
         _snapshot.Parameters.ExistsByName(_parameter.Name).ShouldBeTrue();
      }
   }

   public class When_mapping_a_valid_schema_item_snapshot_to_a_schema_item : concern_for_SchemaItemMapper
   {
      private SchemaItem _newSchemaItem;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_schemaItem);
         A.CallTo(() => _schemaItemFactory.Create(_schemaItem.ApplicationType, null)).Returns(_schemaItem);

         _snapshot.Name = "New Schema Item";
         _snapshot.Description = "The description that will be deserialized";
         _snapshot.FormulationKey = "Toto";
         _snapshot.TargetOrgan = "Tata";
         _snapshot.TargetCompartment = "Titi";
      }

      protected override async Task Because()
      {
         _newSchemaItem = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_have_created_a_schema_item_with_the_expected_properties()
      {
         _newSchemaItem.Name.ShouldBeEqualTo(_snapshot.Name);
         _newSchemaItem.Description.ShouldBeEqualTo(_snapshot.Description);
         _newSchemaItem.FormulationKey.ShouldBeEqualTo(_snapshot.FormulationKey);
         _newSchemaItem.TargetOrgan.ShouldBeEqualTo(_snapshot.TargetOrgan);
         _newSchemaItem.TargetCompartment.ShouldBeEqualTo(_snapshot.TargetCompartment);
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newSchemaItem, PKSimConstants.ObjectTypes.SchemaItem)).MustHaveHappened();
      }
   }
}