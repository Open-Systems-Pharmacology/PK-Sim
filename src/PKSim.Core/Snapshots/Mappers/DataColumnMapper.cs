using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

      public override async Task<SnapshotDataColumn> MapToSnapshot(ModelDataColumn dataColumn)
      {
         var snapshot= await SnapshotFrom(dataColumn, x =>
         {
            x.Name = dataColumn.Name;
            x.Values = valuesInDisplayUnits(dataColumn);
            x.Unit = SnapshotValueFor(dataColumn.DisplayUnit.Name);
         });

         snapshot.RelatedColumns = await mapRelatedColumns(dataColumn.RelatedColumns);
         snapshot.DataInfo = await mapDataInfo(dataColumn.DataInfo);
         snapshot.QuantityInfo = await mapQuantityInfo(dataColumn.QuantityInfo);
         return snapshot;
      }

      private List<float> valuesInDisplayUnits(ModelDataColumn dataColumn)
      {
         return dataColumn.ConvertToDisplayValues(dataColumn.Values).ToList();
      }

      private Task<SnapshotQuantityInfo> mapQuantityInfo(ModelQuantityInfo quantityInfo)
      {
         return _quantityInfoMapper.MapToSnapshot(quantityInfo);
      }

      private Task<DataInfo> mapDataInfo(OSPSuite.Core.Domain.Data.DataInfo dataInfo)
      {
         return _dataInfoMapper.MapToSnapshot(dataInfo);
      }

      private async Task<SnapshotDataColumn[]> mapRelatedColumns(IReadOnlyCollection<ModelDataColumn> relatedColumns)
      {
         if (!relatedColumns.Any())
            return null;

         var tasks = relatedColumns.Select(MapToSnapshot);
         return await Task.WhenAll(tasks);
      }

      public override Task<ModelDataColumn> MapToModel(SnapshotDataColumn snapshot)
      {
         throw new NotImplementedException();
      }
   }
}