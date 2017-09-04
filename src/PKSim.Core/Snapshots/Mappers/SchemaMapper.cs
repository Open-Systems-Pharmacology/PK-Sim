using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelSchema = PKSim.Core.Model.Schema;
using SnapshotSchema = PKSim.Core.Snapshots.Schema;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SchemaMapper : ParameterContainerSnapshotMapperBase<ModelSchema, SnapshotSchema>
   {
      private readonly SchemaItemMapper _schemaItemMapper;
      private readonly ISchemaFactory _schemaFactory;

      public SchemaMapper(ParameterMapper parameterMapper, SchemaItemMapper schemaItemMapper, ISchemaFactory schemaFactory) : base(parameterMapper)
      {
         _schemaItemMapper = schemaItemMapper;
         _schemaFactory = schemaFactory;
      }

      public override SnapshotSchema MapToSnapshot(ModelSchema modelSchema)
      {
         return SnapshotFrom(modelSchema, snapshot => snapshot.SchemaItems.AddRange(snapshotSchemaItemsFrom(modelSchema)));
      }

      private IEnumerable<SchemaItem> snapshotSchemaItemsFrom(ModelSchema modelSchema) => modelSchema.SchemaItems.Select(_schemaItemMapper.MapToSnapshot);

      public override ModelSchema MapToModel(SnapshotSchema snapshotSchema)
      {
         var schema = _schemaFactory.Create();
         MapSnapshotPropertiesIntoModel(snapshotSchema, schema);
         UpdateParametersFromSnapshot(schema, snapshotSchema, PKSimConstants.ObjectTypes.Schema);
         schema.AddChildren(snapshotSchema.SchemaItems.Select(_schemaItemMapper.MapToModel));
         return schema;
      }
   }
}