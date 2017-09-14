using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using SnapshotDataColumn = PKSim.Core.Snapshots.DataColumn;
using SnapshotQuantityInfo = PKSim.Core.Snapshots.QuantityInfo;
using ModelDataColumn = OSPSuite.Core.Domain.Data.DataColumn;
using ModelQuantityInfo = OSPSuite.Core.Domain.Data.QuantityInfo;

namespace PKSim.Core.Snapshots.Mappers
{
   public class DataColumnMapper : SnapshotMapperBase<ModelDataColumn, SnapshotDataColumn>
   {
      private readonly DataInfoMapper _dataInfoMapper;
      private readonly QuantityInfoMapper _quantityInfoMapper;

      public DataColumnMapper(DataInfoMapper dataInfoMapper, QuantityInfoMapper quantityInfoMapper)
      {
         _dataInfoMapper = dataInfoMapper;
         _quantityInfoMapper = quantityInfoMapper;
      }

      public override SnapshotDataColumn MapToSnapshot(ModelDataColumn dataColumn)
      {
         return SnapshotFrom(dataColumn, snapshot =>
         {
            snapshot.Name = dataColumn.Name;
            snapshot.RelatedColumns = mapRelatedColumns(dataColumn.RelatedColumns);
            snapshot.DataInfo = mapDataInfo(dataColumn.DataInfo);
            snapshot.QuantityInfo = mapQuantityInfo(dataColumn.QuantityInfo);
            snapshot.Values = valuesInDisplayUnits(dataColumn);
            snapshot.Unit = SnapshotValueFor(dataColumn.DisplayUnit.Name);
         });
      }

      private List<float> valuesInDisplayUnits(ModelDataColumn dataColumn)
      {
         return dataColumn.ConvertToDisplayValues(dataColumn.Values).ToList();
      }

      private SnapshotQuantityInfo mapQuantityInfo(ModelQuantityInfo quantityInfo)
      {
         return _quantityInfoMapper.MapToSnapshot(quantityInfo);
      }

      private DataInfo mapDataInfo(OSPSuite.Core.Domain.Data.DataInfo dataInfo)
      {
         return _dataInfoMapper.MapToSnapshot(dataInfo);
      }

      private List<SnapshotDataColumn> mapRelatedColumns(IReadOnlyCollection<ModelDataColumn> relatedColumns)
      {
         return relatedColumns.Any() ? relatedColumns.Select(MapToSnapshot).ToList() : null;
      }

      public override ModelDataColumn MapToModel(SnapshotDataColumn snapshot)
      {
         throw new NotImplementedException();
      }
   }
}