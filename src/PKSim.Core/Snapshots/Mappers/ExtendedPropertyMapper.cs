using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using ModelExtendedProperty = OSPSuite.Core.Domain.IExtendedProperty;
using SnapshotExtendedProperty = PKSim.Core.Snapshots.ExtendedProperty;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExtendedPropertyMapper : SnapshotMapperBase<ModelExtendedProperty, SnapshotExtendedProperty>
   {
      public override Task<SnapshotExtendedProperty> MapToSnapshot(ModelExtendedProperty extendedProperty)
      {
         return SnapshotFrom(extendedProperty, snapshot =>
         {
            snapshot.ListOfValues = extendedProperty.ListOfValuesAsObjects.Any() ? extendedProperty.ListOfValuesAsObjects.ToList() : null;
            snapshot.Value = extendedProperty.ValueAsObject;
            snapshot.Description = SnapshotValueFor(extendedProperty.Description);
            snapshot.Name = extendedProperty.Name;
            snapshot.FullName = SnapshotValueFor(extendedProperty.FullName);
            snapshot.ReadOnly = SnapshotValueFor(extendedProperty.ReadOnly);
         });
      }

      private Task<ModelExtendedProperty> mapExtendedProperty<T>(SnapshotExtendedProperty snapshot, Func<object, T> convertToTypeFunc, T propertyValue)
      {
         var extendedProperty = new ExtendedProperty<T>
         {
            Description = snapshot.Description,
            ReadOnly = ModelValueFor(snapshot.ReadOnly),
            FullName = snapshot.FullName,
            Name = snapshot.Name,
            Value = propertyValue
         };
         addOptionsToList(snapshot.ListOfValues, option => extendedProperty.AddToListOfValues(convertToTypeFunc(option)));
         return Task.FromResult<ModelExtendedProperty>(extendedProperty);
      }

      public override Task<ModelExtendedProperty> MapToModel(SnapshotExtendedProperty snapshot)
      {
         if (double.TryParse(snapshot.Value.ToString(), out double doubleResult))
            return mapExtendedProperty(snapshot, option => double.Parse(option.ToString()), doubleResult);

         if (bool.TryParse(snapshot.Value.ToString(), out bool boolResult))
            return mapExtendedProperty(snapshot, option => bool.Parse(option.ToString()), boolResult);


         return mapExtendedProperty(snapshot, option => option.ToString(), snapshot.Value.ToString());
      }

      private void addOptionsToList(List<object> snapshotListOfValues, Action<object> action) => snapshotListOfValues?.Each(action);
   }
}