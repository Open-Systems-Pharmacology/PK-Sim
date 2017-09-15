using System;
using System.Threading.Tasks;
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

      public override Task<ModelDataInfo> MapToModel(SnapshotDataInfo snapshot)
      {
         throw new NotImplementedException();
      }
   }
}