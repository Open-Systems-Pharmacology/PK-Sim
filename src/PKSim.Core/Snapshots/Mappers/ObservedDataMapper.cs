using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Extensions;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;
using SnapshotExtendedProperties = PKSim.Core.Snapshots.ExtendedProperties;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ModelDataColumn = OSPSuite.Core.Domain.Data.DataColumn;
using ModelExtendedProperties = OSPSuite.Core.Domain.ExtendedProperties;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObservedDataMapper : ObjectBaseSnapshotMapperBase<ModelDataRepository, SnapshotDataRepository>
   {
      private readonly ExtendedPropertiesMapper _extendedPropertiesMapper;
      private readonly DataColumnMapper _dataColumnMapper;

      public ObservedDataMapper(ExtendedPropertiesMapper extendedPropertiesMapper, DataColumnMapper dataColumnMapper)
      {
         _extendedPropertiesMapper = extendedPropertiesMapper;
         _dataColumnMapper = dataColumnMapper;
      }

      public override SnapshotDataRepository MapToSnapshot(ModelDataRepository dataRepository)
      {
         return SnapshotFrom(dataRepository, snapshot =>
         {
            snapshot.Name = SnapshotValueFor(dataRepository.Name);
            snapshot.ExtendedProperties = mapExtendedProperties(dataRepository.ExtendedProperties);
            snapshot.Columns = mapColumns(dataRepository.AllButBaseGrid().Where(column => !dataRepository.ColumnIsInRelatedColumns(column)));
            snapshot.BaseGrid = _dataColumnMapper.MapToSnapshot(dataRepository.BaseGrid);
         });
      }

      private List<DataColumn> mapColumns(IEnumerable<ModelDataColumn> dataRepositoryColumns)
      {
         return dataRepositoryColumns.Select(dataColumn => _dataColumnMapper.MapToSnapshot(dataColumn)).ToList();
      }

      private SnapshotExtendedProperties mapExtendedProperties(ModelExtendedProperties extendedProperties)
      {
         return _extendedPropertiesMapper.MapToSnapshot(extendedProperties);
      }

      public override ModelDataRepository MapToModel(SnapshotDataRepository snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}
