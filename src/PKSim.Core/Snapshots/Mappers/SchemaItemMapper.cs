using System.Threading.Tasks;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelSchemaItem = PKSim.Core.Model.SchemaItem;
using SnapshotSchemaItem = PKSim.Core.Snapshots.SchemaItem;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SchemaItemMapper : ParameterContainerSnapshotMapperBase<ModelSchemaItem, SnapshotSchemaItem>
   {
      private readonly ISchemaItemFactory _schemaItemFactory;

      public SchemaItemMapper(ParameterMapper parameterMapper, ISchemaItemFactory schemaItemFactory) : base(parameterMapper)
      {
         _schemaItemFactory = schemaItemFactory;
      }

      public override Task<SnapshotSchemaItem> MapToSnapshot(ModelSchemaItem modelSchemaItem)
      {
         return SnapshotFrom(modelSchemaItem, snapshot =>
         {
            snapshot.ApplicationType = modelSchemaItem.ApplicationType.Name;
            snapshot.FormulationKey = SnapshotValueFor(modelSchemaItem.FormulationKey);
            snapshot.TargetOrgan = SnapshotValueFor(modelSchemaItem.TargetOrgan);
            snapshot.TargetCompartment = SnapshotValueFor(modelSchemaItem.TargetCompartment);
         });
      }

      public override async Task<ModelSchemaItem> MapToModel(SnapshotSchemaItem snapshot)
      {
         var applicationType = ApplicationTypes.ByName(snapshot.ApplicationType);
         var schemaItem = _schemaItemFactory.Create(applicationType);
         MapModelPropertiesToSnapshot(schemaItem, snapshot);
         await UpdateParametersFromSnapshot(snapshot, schemaItem, PKSimConstants.ObjectTypes.SchemaItem);
         schemaItem.FormulationKey = snapshot.FormulationKey;
         schemaItem.TargetOrgan = snapshot.TargetOrgan;
         schemaItem.TargetCompartment = snapshot.TargetCompartment;
         return schemaItem;
      }
   }
}