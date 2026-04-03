using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;
using ModelParameterValue = OSPSuite.Core.Domain.Builder.ParameterValue;
using SnapshotParameterValue = PKSim.Core.Snapshots.ParameterValue;

namespace PKSim.Core.Snapshots.Mappers;

public class ParameterValueMapper : SnapshotMapperBase<ModelParameterValue, SnapshotParameterValue>
{
   public override async Task<SnapshotParameterValue> MapToSnapshot(ModelParameterValue parameterValue)
   {
      // We will only use path and value for this application
      return await SnapshotFrom(parameterValue, snapshot =>
      {
         snapshot.Path = parameterValue.Path.ToString();
         snapshot.Value = parameterValue.Value.GetValueOrDefault();
      });
   }

   public override Task<ModelParameterValue> MapToModel(SnapshotParameterValue snapshot, SnapshotContext snapshotContext)
   {
      var parameterValue = new ModelParameterValue
      {
         Path = snapshot.Path.ToObjectPath(),
         Value = snapshot.Value
      };

      return Task.FromResult(parameterValue);
   }
}
