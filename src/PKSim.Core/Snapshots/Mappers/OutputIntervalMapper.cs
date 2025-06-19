using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;
using System.Threading.Tasks;
using PKSim.Core.Model;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using ModelOutputInterval = OSPSuite.Core.Domain.OutputInterval;
using SnapshotOutputInterval = PKSim.Core.Snapshots.OutputInterval;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputIntervalMapper : ParameterContainerSnapshotMapperBase<ModelOutputInterval, SnapshotOutputInterval>
   {
      private readonly IOutputIntervalFactory _outputIntervalFactory;

      public OutputIntervalMapper(ParameterMapper parameterMapper, IOutputIntervalFactory outputIntervalFactory) : base(parameterMapper)
      {
         _outputIntervalFactory = outputIntervalFactory;
      }

      public override Task<SnapshotOutputInterval> MapToSnapshot(ModelOutputInterval outputInterval)
      {
         return SnapshotFrom(outputInterval, x =>
         {
            //name will be generated on the fly when creating the intervals
            x.Name = null;
         });
      }

      protected override bool ShouldExportToSnapshot(IParameter parameter)
      {
         //we want to ensure that start time and end time are always exported
         return parameter.NameIsOneOf(Constants.Parameters.START_TIME, Constants.Parameters.END_TIME, Constants.Parameters.RESOLUTION) || parameter.ShouldExportToSnapshot();
      }
      public override async Task<ModelOutputInterval> MapToModel(SnapshotOutputInterval snapshot, SnapshotContext snapshotContext)
      {
         var outputInterval = _outputIntervalFactory.CreateDefault();
         await UpdateParametersFromSnapshot(snapshot, outputInterval, snapshotContext, Constants.OUTPUT_INTERVAL);
         return outputInterval;
      }
   }
}