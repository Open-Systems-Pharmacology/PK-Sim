﻿using System.Threading.Tasks;
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
            snapshot.OrderIndex = SnapshotValueFor(quantityInfo.OrderIndex);
            snapshot.Path = SnapshotValueFor(quantityInfo.PathAsString);
            snapshot.Type = quantityInfo.Type != QuantityType.Undefined ? quantityInfo.Type.ToString() : null;
            snapshot.Name = SnapshotValueFor(quantityInfo.Name);
         });
      }

      public override Task<ModelQuantityInfo> MapToModel(SnapshotQuantityInfo snapshot)
      {
         var name = ModelValueFor(snapshot.Name);
         var type = snapshot.Type!=null ? EnumHelper.ParseValue<QuantityType>(snapshot.Type) :  QuantityType.Undefined;

         var path = ModelValueFor(snapshot.Path);

         var quantityInfo = new ModelQuantityInfo(name, path.ToPathArray(), type)
         {
            OrderIndex = ModelValueFor(snapshot.OrderIndex)
         };

         return Task.FromResult(quantityInfo);
      }
   }
}