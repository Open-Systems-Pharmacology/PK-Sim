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
            snapshot.Type = mapExtendedPropertyType(extendedProperty.Type);
         });
      }

      private Task<ModelExtendedProperty> mapExtendedProperty<T>(SnapshotExtendedProperty snapshot, Func<string, T> convertToTypeFunc, T propertyValue)
      {
         var extendedProperty = new ExtendedProperty<T>
         {
            Description = snapshot.Description,
            ReadOnly = ModelValueFor(snapshot.ReadOnly),
            FullName = snapshot.FullName,
            Name = snapshot.Name,
            Value = propertyValue
         };
         addOptionsToList(snapshot.ListOfValues, option => extendedProperty.AddToListOfValues(convertToTypeFunc(option.ToString())));
         return Task.FromResult<ModelExtendedProperty>(extendedProperty);
      }

      private ExtendedPropertyType mapExtendedPropertyType(Type type)
      {
         if (type == typeof(int))
            return ExtendedPropertyType.Integer;

         if (type == typeof(double))
            return ExtendedPropertyType.Double;

         if (type == typeof(bool))
            return ExtendedPropertyType.Boolean;

         return ExtendedPropertyType.String;
      }

      public override Task<ModelExtendedProperty> MapToModel(SnapshotExtendedProperty snapshot, SnapshotContext snapshotContext)
      {
         var snapshotType = snapshot.Type;
         var valueAsString = snapshot.Value.ToString();

         if (snapshotType.HasValue)
            return mapExtendedPropertyBasedOnType(snapshot, snapshotType.Value, valueAsString);

         return mapExtendedPropertyBasedOnValue(snapshot, valueAsString);
      }

      private Task<ModelExtendedProperty> mapExtendedPropertyBasedOnType(ExtendedProperty snapshot, ExtendedPropertyType snapshotType, string valueAsString)
      {
         switch (snapshotType)
         {
            case ExtendedPropertyType.Integer:
               return mapExtendedProperty(snapshot, int.Parse, int.Parse(valueAsString));
            case ExtendedPropertyType.Double:
               return mapExtendedProperty(snapshot, double.Parse, double.Parse(valueAsString));
            case ExtendedPropertyType.Boolean:
               return mapExtendedProperty(snapshot, bool.Parse, bool.Parse(valueAsString));
            //string
            default:
               return mapExtendedProperty(snapshot, option => option, valueAsString);
         }
      }

      private Task<ModelExtendedProperty> mapExtendedPropertyBasedOnValue(ExtendedProperty snapshot, string valueAsString)
      {
         if (double.TryParse(valueAsString, out var doubleResult))
            return mapExtendedProperty(snapshot, double.Parse, doubleResult);

         if (bool.TryParse(valueAsString, out var boolResult))
            return mapExtendedProperty(snapshot, bool.Parse, boolResult);

         return mapExtendedProperty(snapshot, option => option, valueAsString);
      }

      private void addOptionsToList(List<object> snapshotListOfValues, Action<object> action) => snapshotListOfValues?.Each(action);

      public void ClearDatabaseProperties(SnapshotExtendedProperty extendedProperty)
      {
         if (extendedProperty == null)
            return;

         extendedProperty.Description = null;
         extendedProperty.FullName = null;
      }
   }
}