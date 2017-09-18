using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;
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
            snapshot.Type = quantityInfo.Type.ToString();
            snapshot.Name = quantityInfo.Name;
         });
      }

      public override Task<ModelQuantityInfo> MapToModel(SnapshotQuantityInfo snapshot)
      {
         var modelQuantityInfo = new ModelQuantityInfo(snapshot.Name, snapshot.Path.ToPathArray(), EnumHelper.ParseValue<QuantityType>(snapshot.Type))
         {
            OrderIndex = snapshot.OrderIndex
         };

         return Task.FromResult(modelQuantityInfo);
      }
   }
}