using System.Threading.Tasks;
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
            snapshot.OrderIndex = quantityInfo.OrderIndex;
            snapshot.Path = quantityInfo.PathAsString;
            snapshot.Type = quantityInfo.Type;
            snapshot.Name = quantityInfo.Name;
         });
      }

      public override Task<ModelQuantityInfo> MapToModel(SnapshotQuantityInfo snapshot)
      {
         var modelQuantityInfo = new ModelQuantityInfo(snapshot.Name, snapshot.Path.ToPathArray(), snapshot.Type)
         {
            OrderIndex = snapshot.OrderIndex
         };

         return Task.FromResult(modelQuantityInfo);
      }
   }
}