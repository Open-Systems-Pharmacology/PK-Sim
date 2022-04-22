using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using ModelQuantityInfo = OSPSuite.Core.Domain.Data.QuantityInfo;
using SnapshotQuantityInfo = PKSim.Core.Snapshots.QuantityInfo;

namespace PKSim.Core.Snapshots.Mappers
{
   public class QuantityInfoMapper : SnapshotMapperBase<ModelQuantityInfo, SnapshotQuantityInfo>
   {
      public override Task<SnapshotQuantityInfo> MapToSnapshot(ModelQuantityInfo quantityInfo)
      {
         return SnapshotFrom(quantityInfo, snapshot =>
         {
            snapshot.OrderIndex = SnapshotValueFor(quantityInfo.OrderIndex);
            snapshot.Path = SnapshotValueFor(quantityInfo.PathAsString);
            snapshot.Type = SnapshotValueFor(quantityInfo.Type, QuantityType.Undefined);
         });
      }

      public override Task<ModelQuantityInfo> MapToModel(SnapshotQuantityInfo snapshot, SnapshotContext snapshotContext)
      {
         var type = ModelValueFor(snapshot.Type, QuantityType.Undefined);
         var path = ModelValueFor(snapshot.Path);

         var quantityInfo = new ModelQuantityInfo(path.ToPathArray(), type)
         {
            OrderIndex = ModelValueFor(snapshot.OrderIndex)
         };

         return Task.FromResult(quantityInfo);
      }
   }
}