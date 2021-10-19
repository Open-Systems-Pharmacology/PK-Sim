using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
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
         snapshot.SchemaItems = await _schemaItemMapper.MapToSnapshots(modelSchema.SchemaItems);
         return snapshot;
      }

      protected override bool ShouldExportParameterToSnapshot(IParameter parameter)
      {
         //we want to ensure that start time and end time are always exported
         return parameter.NameIsOneOf(Constants.Parameters.START_TIME, CoreConstants.Parameters.NUMBER_OF_REPETITIONS,
            CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS) || base.ShouldExportParameterToSnapshot(parameter);
      }

      public override async Task<ModelSchema> MapToModel(SnapshotSchema snapshot)
      {
         var schema = _schemaFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, schema);
         await UpdateParametersFromSnapshot(snapshot, schema, PKSimConstants.ObjectTypes.Schema);
         var schemaItems = await _schemaItemMapper.MapToModels(snapshot.SchemaItems);
         schemaItems?.Each(schema.AddSchemaItem);
         return schema;
      }
   }
}