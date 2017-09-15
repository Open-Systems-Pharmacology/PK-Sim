using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using SnapshotOutputSchema = PKSim.Core.Snapshots.OutputSchema;
using ModelOutputSchema = OSPSuite.Core.Domain.OutputSchema;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputSchemaMapper : SnapshotMapperBase<ModelOutputSchema, SnapshotOutputSchema>
   {
      private readonly OutputIntervalMapper _outputIntervalMapper;

      public OutputSchemaMapper(OutputIntervalMapper outputIntervalMapper)
      {
         _outputIntervalMapper = outputIntervalMapper;
      }

      public override async Task<SnapshotOutputSchema> MapToSnapshot(ModelOutputSchema outputSchema)
      {
         var snapshot = await SnapshotFrom(outputSchema);
         var intervals = await allSnapshotIntervalsFor(outputSchema);
         intervals.Each(snapshot.Add);
         return snapshot;
      }

      private Task<OutputInterval[]> allSnapshotIntervalsFor(ModelOutputSchema outputSchema)
      {
         var tasks = outputSchema.Intervals.Select(_outputIntervalMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      public override Task<ModelOutputSchema> MapToModel(SnapshotOutputSchema snapshot)
      {
         throw new NotImplementedException();
      }
   }
}