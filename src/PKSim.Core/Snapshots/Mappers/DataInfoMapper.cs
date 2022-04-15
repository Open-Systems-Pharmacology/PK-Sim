using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using SnapshotDataInfo = PKSim.Core.Snapshots.DataInfo;
using ModelDataInfo = OSPSuite.Core.Domain.Data.DataInfo;

namespace PKSim.Core.Snapshots.Mappers
{
   public class DataInfoMapper : SnapshotMapperBase<ModelDataInfo, SnapshotDataInfo>
   {
      private readonly ExtendedPropertyMapper _extendedPropertyMapper;
      private readonly IDimension _molWeightDimension;

      public DataInfoMapper(ExtendedPropertyMapper extendedPropertyMapper, IDimensionRepository dimensionRepository)
      {
         _molWeightDimension = dimensionRepository.DimensionByName(Constants.Dimension.MOLECULAR_WEIGHT);
         _extendedPropertyMapper = extendedPropertyMapper;
      }

      public override async Task<SnapshotDataInfo> MapToSnapshot(ModelDataInfo dataInfo)
      {
         var snapshot = await SnapshotFrom(dataInfo, x =>
         {
            x.AuxiliaryType = dataInfo.AuxiliaryType;
            x.Category = SnapshotValueFor(dataInfo.Category);
            x.ComparisonThreshold = dataInfo.ComparisonThreshold;
            x.Date = dataInfo.Date;
            x.LLOQ = dataInfo.LLOQ;
            x.MolWeight = molWeightToDisplayValue(dataInfo);
            x.Origin = SnapshotValueFor(dataInfo.Origin, ColumnOrigins.Undefined);
            x.Source = SnapshotValueFor(dataInfo.Source);
         });
         snapshot.ExtendedProperties = await _extendedPropertyMapper.MapToSnapshots(dataInfo.ExtendedProperties);
         return snapshot;
      }

      private double? molWeightToDisplayValue(ModelDataInfo dataInfo)
      {
         if (dataInfo.MolWeight != null)
            return _molWeightDimension.BaseUnitValueToUnitValue(_molWeightDimension.DefaultUnit, dataInfo.MolWeight.Value);
         return null;
      }

      public override async Task<ModelDataInfo> MapToModel(SnapshotDataInfo snapshot, SnapshotContext snapshotContext)
      {
         var origin = ModelValueFor(snapshot.Origin, ColumnOrigins.Undefined);
         var dataInfo = new ModelDataInfo(origin)
         {
            AuxiliaryType = snapshot.AuxiliaryType,
            Category = snapshot.Category,
            ComparisonThreshold = snapshot.ComparisonThreshold,
            Date = snapshot.Date,
            LLOQ = snapshot.LLOQ,
            MolWeight = molWeightToBaseValue(snapshot),
            Source = ModelValueFor(snapshot.Source)
         };

         var extendedProperties = await _extendedPropertyMapper.MapToModels(snapshot.ExtendedProperties, snapshotContext);
         extendedProperties?.Each(dataInfo.ExtendedProperties.Add);

         return dataInfo;
      }

      private double? molWeightToBaseValue(SnapshotDataInfo snapshot)
      {
         if (snapshot.MolWeight == null)
            return null;

         return _molWeightDimension.UnitValueToBaseUnitValue(_molWeightDimension.DefaultUnit, snapshot.MolWeight.Value);
      }
   }
}