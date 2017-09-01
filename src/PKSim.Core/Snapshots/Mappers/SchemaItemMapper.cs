using ModelSchemaItem = PKSim.Core.Model.SchemaItem;
using SnapshotSchemaItem = PKSim.Core.Snapshots.SchemaItem;


namespace PKSim.Core.Snapshots.Mappers
{
   public class SchemaItemMapper : ParameterContainerSnapshotMapperBase<ModelSchemaItem, SnapshotSchemaItem>
   {
      public SchemaItemMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override SnapshotSchemaItem MapToSnapshot(ModelSchemaItem modelSchemaItem)
      {
         var snapshot = CreateSnapshotWithDefaultPropertiesFor(modelSchemaItem);
         snapshot.ApplicationType = modelSchemaItem.ApplicationType.Name;
         snapshot.FormulationKey = SnapshotValueFor(modelSchemaItem.FormulationKey);
         snapshot.TargetOrgan = SnapshotValueFor(modelSchemaItem.TargetOrgan);
         snapshot.TargetCompartment = SnapshotValueFor(modelSchemaItem.TargetCompartment);
         return snapshot;
      }

      public override ModelSchemaItem MapToModel(SchemaItem snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}