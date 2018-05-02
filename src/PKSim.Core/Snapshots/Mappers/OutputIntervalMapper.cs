using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using SnapshotOutputInterval = PKSim.Core.Snapshots.OutputInterval;
using ModelOutputInterval = OSPSuite.Core.Domain.OutputInterval;

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

      protected override bool ShouldExportParameterToSnapshot(IParameter parameter)
      {
         //we want to ensure that start time and end time are always exported
         return parameter.NameIsOneOf(Constants.Parameters.START_TIME, Constants.Parameters.END_TIME, Constants.Parameters.RESOLUTION) || base.ShouldExportParameterToSnapshot(parameter);
      }

      public override async Task<ModelOutputInterval> MapToModel(SnapshotOutputInterval snapshot)
      {
         var outputInterval = _outputIntervalFactory.CreateDefault();
         await UpdateParametersFromSnapshot(snapshot, outputInterval, Constants.OUTPUT_INTERVAL);
         return outputInterval;
      }
   }
}