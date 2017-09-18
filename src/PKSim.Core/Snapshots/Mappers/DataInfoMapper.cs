﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Utility;
using SnapshotDataInfo = PKSim.Core.Snapshots.DataInfo;
using ModelDataInfo = OSPSuite.Core.Domain.Data.DataInfo;
using ModelExtendedProperties = OSPSuite.Core.Domain.ExtendedProperties;
using SnapshotExtendedProperties = PKSim.Core.Snapshots.ExtendedProperties;

namespace PKSim.Core.Snapshots.Mappers
{
   public class DataInfoMapper : SnapshotMapperBase<ModelDataInfo, SnapshotDataInfo>
   {
      private readonly ExtendedPropertiesMapper _extendedPropertiesMapper;

      public DataInfoMapper(ExtendedPropertiesMapper extendedPropertiesMapper)
      {
         _extendedPropertiesMapper = extendedPropertiesMapper;
      }

      public override async Task<SnapshotDataInfo> MapToSnapshot(ModelDataInfo dataInfo)
      {
         var snapshot = await SnapshotFrom(dataInfo, x =>
         {
            x.AuxiliaryType = dataInfo.AuxiliaryType.ToString();
            x.Category = dataInfo.Category;
            x.ComparisonThreshold = dataInfo.ComparisonThreshold;
            x.Date = dataInfo.Date;
            x.LLOQ = dataInfo.LLOQ;
            x.MolWeight = dataInfo.MolWeight;
            x.Origin = dataInfo.Origin.ToString();
            x.Source = dataInfo.Source;
         });
         snapshot.ExtendedProperties = await mapExtendedProperties(dataInfo.ExtendedProperties);
         return snapshot;
      }

      private Task<SnapshotExtendedProperties> mapExtendedProperties(ModelExtendedProperties extendedProperties)
      {
         return _extendedPropertiesMapper.MapToSnapshot(extendedProperties);
      }

      public override async Task<ModelDataInfo> MapToModel(SnapshotDataInfo snapshot)
      {
         var dataInfo = new ModelDataInfo(EnumHelper.ParseValue<ColumnOrigins>(snapshot.Origin))
         {
            AuxiliaryType = EnumHelper.ParseValue<AuxiliaryType>(snapshot.AuxiliaryType),
            Category = snapshot.Category,
            ComparisonThreshold = snapshot.ComparisonThreshold,
            Date = snapshot.Date,
            LLOQ = snapshot.LLOQ,
            MolWeight = snapshot.MolWeight,
            Source = snapshot.Source,
         };

         if(snapshot.ExtendedProperties != null)
            dataInfo.ExtendedProperties.AddRange(await extendedPropertiesFrom(snapshot));

         return dataInfo;
      }

      private async Task<IEnumerable<IExtendedProperty>> extendedPropertiesFrom(SnapshotDataInfo snapshot)
      {
         return await _extendedPropertiesMapper.MapToModel(snapshot.ExtendedProperties);
      }
   }
}