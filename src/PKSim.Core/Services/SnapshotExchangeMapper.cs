using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Services;

public abstract class SnapshotExchangeMapper<TBuildingBlock, TSnapshot, TSnapshotModel>(IJsonSerializer jsonSerializer, ISnapshotMapper snapshotMapper) : IMapper<string, TBuildingBlock>
   where TSnapshot : class
   where TSnapshotModel : class
{
   public abstract TBuildingBlock MapFrom(string input);

   private TSnapshot deserializeFromBase64Snapshot(string snapshotString) => jsonSerializer.DeserializeFromBase64String<TSnapshot>(snapshotString).Result;

   protected TSnapshotModel PKSimModelFor(string individualSnapshotString)
   {
      var snapshot = deserializeFromBase64Snapshot(individualSnapshotString);
      return snapshotMapper.MapToModel(snapshot, new ProjectContext(new PKSimProject(), runSimulations: false)).Result as TSnapshotModel;
   }
}