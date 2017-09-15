using System.Threading.Tasks;
using SnapshotOutputInterval = PKSim.Core.Snapshots.OutputInterval;
using ModelOutputInterval = OSPSuite.Core.Domain.OutputInterval;


namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputIntervalMapper : ParameterContainerSnapshotMapperBase<ModelOutputInterval, SnapshotOutputInterval>
   {
      public OutputIntervalMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override Task<SnapshotOutputInterval> MapToSnapshot(ModelOutputInterval outputInterval)
      {
         return SnapshotFrom(outputInterval, x =>
         {
            //name will be generated on the fly when creating the intervals
            x.Name = null;
         });
      }

      public override Task<ModelOutputInterval> MapToModel(SnapshotOutputInterval snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}