using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using SnapshotDataColumn = PKSim.Core.Snapshots.DataColumn;
using SnapshotQuantityInfo = PKSim.Core.Snapshots.QuantityInfo;
using ModelDataColumn = OSPSuite.Core.Domain.Data.DataColumn;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ModelQuantityInfo = OSPSuite.Core.Domain.Data.QuantityInfo;

namespace PKSim.Core.Snapshots.Mappers
{
   public class DataColumnMapper : SnapshotMapperBase<ModelDataColumn, SnapshotDataColumn, ModelDataRepository>
   {
      private readonly DataInfoMapper _dataInfoMapper;
      private readonly QuantityInfoMapper _quantityInfoMapper;
      private readonly IDimensionRepository _dimensionRepository;

      public DataColumnMapper(DataInfoMapper dataInfoMapper, QuantityInfoMapper quantityInfoMapper, IDimensionRepository dimensionRepository)
      {
         _dataInfoMapper = dataInfoMapper;
         _quantityInfoMapper = quantityInfoMapper;
         _dimensionRepository = dimensionRepository;
      }

      public override async Task<SnapshotDataColumn> MapToSnapshot(ModelDataColumn dataColumn)
      {
         var snapshot = await SnapshotFrom(dataColumn, x =>
         {
            x.Name = dataColumn.Name;
            x.Values = valuesInDisplayUnits(dataColumn);
            x.Dimension = dataColumn.Dimension.Name;
            x.Unit = SnapshotValueFor(dataColumn.DisplayUnit.Name);
         });

         snapshot.RelatedColumns = await mapRelatedColumns(dataColumn.RelatedColumns);
         snapshot.DataInfo = await mapDataInfo(dataColumn.DataInfo);
         snapshot.QuantityInfo = await mapQuantityInfo(dataColumn.QuantityInfo);
         return snapshot;
      }

      private IReadOnlyList<float> valuesInBaseUnits(ModelDataColumn dataColumn, IEnumerable<float> valuesInDisplayUnits)
      {
         return dataColumn.ConvertToBaseValues(valuesInDisplayUnits);
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

      public override async Task<ModelDataColumn> MapToModel(SnapshotDataColumn snapshot, ModelDataRepository dataRepository)
      {
         var dimension = dimensionFrom(snapshot);
         var dataColumn = snapshot.IsBaseGrid ? new BaseGrid(snapshot.Name, dimension) : new ModelDataColumn(snapshot.Name, dimension, dataRepository.BaseGrid);
         dataColumn.DisplayUnit = displayUnitFor(dimension, snapshot.Unit);

         dataColumn.Values = valuesInBaseUnits(dataColumn, snapshot.Values);
         dataColumn.DataInfo = await _dataInfoMapper.MapToModel(snapshot.DataInfo);
         dataColumn.QuantityInfo = await _quantityInfoMapper.MapToModel(snapshot.QuantityInfo);
         var tasks = snapshot.RelatedColumns?.Select(relatedColumn => MapToModel(relatedColumn, dataRepository));

         if (tasks == null)
            return dataColumn;

         var relatedColumns = await Task.WhenAll(tasks);
         relatedColumns.Each(dataColumn.AddRelatedColumn);

         return dataColumn;
      }

      private Unit displayUnitFor(IDimension dimension, string snapshotUnit)
      {
         return dimension.UnitOrDefault(snapshotUnit);
      }

      private IDimension dimensionFrom(SnapshotDataColumn snapshot)
      {
         return _dimensionRepository.DimensionByName(snapshot.Dimension);
      }
   }
}