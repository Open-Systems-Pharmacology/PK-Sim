using System.Collections.Generic;
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

         var snapshot =  await SnapshotFrom(extendedProperties);
         var snapshotExtendedProperties =await mapExtendedPropertiesFrom(extendedProperties);
         snapshotExtendedProperties.Each(snapshot.Add);
         return snapshot;
      }

      private Task<SnapshotExtendedProperty[]> mapExtendedPropertiesFrom(ModelExtendedProperties extendedProperties)
      {
         var tasks = extendedProperties.Select(_extendedPropertyMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      public override async Task<ModelExtendedProperties> MapToModel(SnapshotExtendedProperties snapshot)
      {
         var extendedProperties = new ModelExtendedProperties();
         var tasks = snapshot.Select(_extendedPropertyMapper.MapToModel);
         extendedProperties.AddRange(await Task.WhenAll(tasks));
         return extendedProperties;
      }
   }
}
