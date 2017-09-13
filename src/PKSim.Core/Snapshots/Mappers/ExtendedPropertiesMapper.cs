using System.Collections.Generic;
using System.Linq;
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

      public override SnapshotExtendedProperties MapToSnapshot(ModelExtendedProperties extendedProperties)
      {
         return SnapshotFrom(extendedProperties, snapshot =>
         {
            snapshot.ListOfExtendedProperties = mapExtendedPropertiesFrom(extendedProperties);
         });
      }

      private List<SnapshotExtendedProperty> mapExtendedPropertiesFrom(ModelExtendedProperties extendedProperties)
      {
         return extendedProperties.Any() ? extendedProperties.Select(property => _extendedPropertyMapper.MapToSnapshot(property)).ToList() : null;
      }

      public override ModelExtendedProperties MapToModel(SnapshotExtendedProperties snapshot)
      {
         var extendedProperties = new ModelExtendedProperties();

         extendedProperties.AddRange(snapshot.ListOfExtendedProperties.Select(propertySnapshot => _extendedPropertyMapper.MapToModel(propertySnapshot)));
         return extendedProperties;
      }
   }
}
