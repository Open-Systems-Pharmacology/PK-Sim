using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

      public override async Task<SnapshotDataRepository> MapToSnapshot(ModelDataRepository dataRepository)
      {
         var snapshot = await SnapshotFrom(dataRepository, x => { x.Name = SnapshotValueFor(dataRepository.Name); });

         snapshot.ExtendedProperties = await mapExtendedProperties(dataRepository.ExtendedProperties);
         snapshot.Columns = await mapColumns(dataRepository.AllButBaseGrid().Where(column => !dataRepository.ColumnIsInRelatedColumns(column)));
         snapshot.BaseGrid = await _dataColumnMapper.MapToSnapshot(dataRepository.BaseGrid);
         return snapshot;
      }

      private Task<DataColumn[]> mapColumns(IEnumerable<ModelDataColumn> dataRepositoryColumns)
      {
         return _dataColumnMapper.MapToSnapshots(dataRepositoryColumns);
      }

      private Task<SnapshotExtendedProperties> mapExtendedProperties(ModelExtendedProperties extendedProperties)
      {
         return _extendedPropertiesMapper.MapToSnapshot(extendedProperties);
      }

      public override async Task<ModelDataRepository> MapToModel(SnapshotDataRepository snapshot)
      {
         var dataRepository = new ModelDataRepository();
         MapSnapshotPropertiesToModel(snapshot, dataRepository);

         dataRepository.Add(await _dataColumnMapper.MapToModel(snapshot.BaseGrid, dataRepository));
         dataRepository.AddColumns(await _dataColumnMapper.MapToModels(snapshot.Columns, dataRepository) );
         dataRepository.ExtendedProperties.AddRange(await _extendedPropertiesMapper.MapToModel(snapshot.ExtendedProperties));

         return dataRepository;
      }
   }
}