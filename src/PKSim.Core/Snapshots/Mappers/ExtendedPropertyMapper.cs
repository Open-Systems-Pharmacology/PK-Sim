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
            snapshot.Type = extendedProperty.Type;
            snapshot.Description = SnapshotValueFor(extendedProperty.Description);
            snapshot.Name = extendedProperty.Name;
            snapshot.FullName = extendedProperty.FullName;
            snapshot.DisplayName = extendedProperty.DisplayName;
            snapshot.ReadOnly = extendedProperty.ReadOnly;
         });
      }

      private Task<ModelExtendedProperty> mapExtendedProperty<T>(SnapshotExtendedProperty snapshot, Func<object, T> convertToTypeFunc)
      {
         var extendedProperty = new ExtendedProperty<T>
         {
            Description = snapshot.Description,
            ReadOnly = snapshot.ReadOnly,
            FullName = snapshot.FullName,
            Name = snapshot.Name,
            ValueAsObject = snapshot.Value
         };
         addOptionsToList(snapshot.ListOfValues, option => extendedProperty.AddToListOfValues(convertToTypeFunc(option)));
         return Task.FromResult<ModelExtendedProperty>(extendedProperty);
      }

      public override Task<ModelExtendedProperty> MapToModel(SnapshotExtendedProperty snapshot)
      {
         if (snapshot.Type == typeof(string))
            return mapExtendedProperty(snapshot, option => option.ToString());

         if (snapshot.Type == typeof(double))
            return mapExtendedProperty(snapshot, option => double.Parse(option.ToString()));

         if (snapshot.Type == typeof(bool))
            return mapExtendedProperty(snapshot, option => bool.Parse(option.ToString()));

         return mapExtendedProperty(snapshot, option => option);
      }

      private void addOptionsToList(List<object> snapshotListOfValues, Action<object> action) => snapshotListOfValues.Each(action);
   }
}