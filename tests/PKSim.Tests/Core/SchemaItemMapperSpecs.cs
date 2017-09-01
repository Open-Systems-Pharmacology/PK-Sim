using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaItemMapper : ContextSpecification<SchemaItemMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected SchemaItem _schemaItem;
      protected Snapshots.SchemaItem _snapshot;
      protected IParameter _parameter1;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();

         sut = new SchemaItemMapper(_parameterMapper);

         _schemaItem = new SchemaItem
         {
            Name = "SchemaItem",
            Description = "The schema item",
            ApplicationType = ApplicationTypes.Intravenous,
            FormulationKey = "F1",
            TargetCompartment = "Cells",
            TargetOrgan = "Liver"
         };

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("Param1");
         _schemaItem.Add(_parameter1);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Parameter().WithName(_parameter1.Name));
      }
   }

   public class When_mapping_a_model_schema_item_to_a_snapshot_schema_item : concern_for_SchemaItemMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_schemaItem);
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
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
      }
   }
}