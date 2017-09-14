using System.Linq;
using System.Threading.Tasks;
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

      public override async Task<SnapshotSchema> MapToSnapshot(ModelSchema modelSchema)
      {
         var snapshot = await SnapshotFrom(modelSchema);
         snapshot.SchemaItems.AddRange(await snapshotSchemaItemsFrom(modelSchema));
         return snapshot;
      }

      private Task<SchemaItem[]> snapshotSchemaItemsFrom(ModelSchema modelSchema)
      {
         var tasks = modelSchema.SchemaItems.Select(_schemaItemMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      public override async Task<ModelSchema> MapToModel(SnapshotSchema snapshotSchema)
      {
         var schema = _schemaFactory.Create();
         MapSnapshotPropertiesToModel(snapshotSchema, schema);
         await UpdateParametersFromSnapshot(snapshotSchema, schema, PKSimConstants.ObjectTypes.Schema);
         var tasks = snapshotSchema.SchemaItems.Select(_schemaItemMapper.MapToModel);
         schema.AddChildren(await Task.WhenAll(tasks));
         return schema;
      }
   }
}