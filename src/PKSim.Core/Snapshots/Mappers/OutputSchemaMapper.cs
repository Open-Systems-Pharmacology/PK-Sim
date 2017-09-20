using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotOutputSchema = PKSim.Core.Snapshots.OutputSchema;
using ModelOutputSchema = OSPSuite.Core.Domain.OutputSchema;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputSchemaMapper : SnapshotMapperBase<ModelOutputSchema, SnapshotOutputSchema>
   {
      private readonly OutputIntervalMapper _outputIntervalMapper;
      private readonly IOutputSchemaFactory _outputSchemaFactory;
      private readonly IContainerTask _containerTask;

      public OutputSchemaMapper(OutputIntervalMapper outputIntervalMapper, IOutputSchemaFactory outputSchemaFactory, IContainerTask _containerTask)
      {
         _outputIntervalMapper = outputIntervalMapper;
         _outputSchemaFactory = outputSchemaFactory;
         this._containerTask = _containerTask;
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

      public override async Task<ModelOutputSchema> MapToModel(SnapshotOutputSchema snapshot)
      {
         var outputSchema = _outputSchemaFactory.CreateEmpty();
         var tasks = snapshot.Select(x=>_outputIntervalMapper.MapToModel(x));
         var intervals = await Task.WhenAll(tasks);
         intervals.Each(interval =>
         {
            interval.Name = _containerTask.CreateUniqueName(outputSchema, interval.Name);
            outputSchema.AddInterval(interval);
         });
         return outputSchema;
      }
   }
}