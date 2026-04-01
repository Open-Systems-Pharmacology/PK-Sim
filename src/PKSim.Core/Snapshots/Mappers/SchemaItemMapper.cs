using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Assets;
using PKSim.Core.Model;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using ModelSchemaItem = PKSim.Core.Model.SchemaItem;
using SnapshotSchemaItem = PKSim.Core.Snapshots.SchemaItem;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SchemaItemMapper : ParameterContainerSnapshotMapperBase<ModelSchemaItem, SnapshotSchemaItem, SnapshotContext>
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
            snapshot.EventKey = SnapshotValueFor(modelSchemaItem.EventKey);
         });
      }

      public override async Task<ModelSchemaItem> MapToModel(SnapshotSchemaItem snapshot, SnapshotContext snapshotContext)
      {
         var applicationType = ApplicationTypes.ByName(snapshot.ApplicationType);
         var schemaItem = applicationType == ApplicationTypes.Event
            ? _schemaItemFactory.CreateEvent(snapshot.EventKey)
            : _schemaItemFactory.Create(applicationType);

         MapSnapshotPropertiesToModel(snapshot, schemaItem);
         await UpdateParametersFromSnapshot(snapshot, schemaItem, snapshotContext, PKSimConstants.ObjectTypes.SchemaItem);
         schemaItem.FormulationKey = snapshot.FormulationKey;
         schemaItem.TargetOrgan = snapshot.TargetOrgan;
         schemaItem.TargetCompartment = snapshot.TargetCompartment;
         schemaItem.EventKey = snapshot.EventKey;
         return schemaItem;
      }

      protected override bool ShouldExportToSnapshot(IParameter parameter) => parameter.ShouldExportToSnapshot();
   }
}