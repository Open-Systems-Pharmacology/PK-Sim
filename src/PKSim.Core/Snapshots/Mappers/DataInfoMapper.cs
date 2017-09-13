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

      public override SnapshotDataInfo MapToSnapshot(ModelDataInfo dataInfo)
      {
         return SnapshotFrom(dataInfo, snapshot =>
         {
            snapshot.AuxiliaryType = dataInfo.AuxiliaryType.ToString();
            snapshot.Category = dataInfo.Category;
            snapshot.ComparisonThreshold = dataInfo.ComparisonThreshold;
            snapshot.Date = dataInfo.Date;
            snapshot.DisplayUnitName = dataInfo.DisplayUnitName;
            snapshot.ExtendedProperties = mapExtendedProperties(dataInfo.ExtendedProperties);
            snapshot.LLOQ = dataInfo.LLOQ;
            snapshot.MolWeight = dataInfo.MolWeight;
            snapshot.Origin = dataInfo.Origin.ToString();
            snapshot.Source = dataInfo.Source;
         });
      }

      private SnapshotExtendedProperties mapExtendedProperties(ModelExtendedProperties extendedProperties)
      {
         return _extendedPropertiesMapper.MapToSnapshot(extendedProperties);
      }

      public override ModelDataInfo MapToModel(SnapshotDataInfo snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}