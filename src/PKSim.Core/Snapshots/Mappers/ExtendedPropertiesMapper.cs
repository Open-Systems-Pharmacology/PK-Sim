using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using SnapshotExtendedProperties = PKSim.Core.Snapshots.ExtendedProperties;
using ModelExtendedProperties = OSPSuite.Core.Domain.ExtendedProperties;
using SnapshotExtendedProperty = PKSim.Core.Snapshots.ExtendedProperty;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExtendedPropertiesMapper : SnapshotMapperBase<ModelExtendedProperties, SnapshotExtendedProperties>
   {
      private readonly ExtendedPropertyMapper _extendedPropertyMapper;

      public ExtendedPropertiesMapper(ExtendedPropertyMapper extendedPropertyMapper)
      {
         _extendedPropertyMapper = extendedPropertyMapper;
      }

      public override async Task<SnapshotExtendedProperties> MapToSnapshot(ModelExtendedProperties extendedProperties)
      {
         if (!extendedProperties.Any())
            return null;

         var snapshot = await SnapshotFrom(extendedProperties);
         var snapshotExtendedProperties = await mapExtendedPropertiesFrom(extendedProperties);
         snapshotExtendedProperties.Each(snapshot.Add);
         return snapshot;
      }

      private Task<SnapshotExtendedProperty[]> mapExtendedPropertiesFrom(ModelExtendedProperties extendedProperties)
      {
         return _extendedPropertyMapper.MapToSnapshots(extendedProperties);
      }

      public override async Task<ModelExtendedProperties> MapToModel(SnapshotExtendedProperties snapshot)
      {
         var extendedProperties = new ModelExtendedProperties();
         var properties = await _extendedPropertyMapper.MapToModels(snapshot);
         extendedProperties.AddRange(properties);
         return extendedProperties;
      }
   }
}