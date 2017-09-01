using System;
using System.Linq;
using ModelSchema = PKSim.Core.Model.Schema;
using SnapshotSchema = PKSim.Core.Snapshots.Schema;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SchemaMapper : ParameterContainerSnapshotMapperBase<ModelSchema, SnapshotSchema>
   {
      private readonly SchemaItemMapper _schemaItemMapper;

      public SchemaMapper(ParameterMapper parameterMapper, SchemaItemMapper schemaItemMapper) : base(parameterMapper)
      {
         _schemaItemMapper = schemaItemMapper;
      }

      public override SnapshotSchema MapToSnapshot(ModelSchema modelSchema)
      {
         var snapshot = CreateSnapshotWithDefaultPropertiesFor(modelSchema);
         snapshot.SchemaItems.AddRange(modelSchema.SchemaItems.Select(snapshotSchemaItemFrom));
         return snapshot;
      }

      private SchemaItem snapshotSchemaItemFrom(Model.SchemaItem schemaItem) => _schemaItemMapper.MapToSnapshot(schemaItem);

      public override ModelSchema MapToModel(SnapshotSchema snapshotSchema)
      {
         throw new NotImplementedException();
      }
   }
}