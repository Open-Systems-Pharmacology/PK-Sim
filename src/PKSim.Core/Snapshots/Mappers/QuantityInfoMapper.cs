using System.Linq;
using ModelQuantityInfo = OSPSuite.Core.Domain.Data.QuantityInfo;
using SnapshotQuantityInfo = PKSim.Core.Snapshots.QuantityInfo;

namespace PKSim.Core.Snapshots.Mappers
{
   public class QuantityInfoMapper : SnapshotMapperBase<ModelQuantityInfo, SnapshotQuantityInfo>
   {
      public override SnapshotQuantityInfo MapToSnapshot(ModelQuantityInfo quantityInfo)
      {
         return SnapshotFrom(quantityInfo, snapshot =>
         {
            snapshot.OrderIndex = quantityInfo.OrderIndex;
            snapshot.Path = quantityInfo.Path.ToList();
            snapshot.Type = quantityInfo.Type.ToString();
            snapshot.Name = quantityInfo.Name;
         });
      }

      public override ModelQuantityInfo MapToModel(SnapshotQuantityInfo snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}