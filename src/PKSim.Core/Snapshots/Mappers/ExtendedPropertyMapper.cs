using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using ModelExtendedProperty = OSPSuite.Core.Domain.IExtendedProperty;
using SnapshotExtendedProperty = PKSim.Core.Snapshots.ExtendedProperty;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExtendedPropertyMapper : SnapshotMapperBase<ModelExtendedProperty, SnapshotExtendedProperty>
   {
      public override SnapshotExtendedProperty MapToSnapshot(ModelExtendedProperty extendedProperty)
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

      private ExtendedProperty<T> addOptionsToExtendedProperty<T>(SnapshotExtendedProperty snapshot, Func<object, T> convertToTypeFunc)
      {
         var extendedProperty = new ExtendedProperty<T>();
         addOptionsToList(snapshot.ListOfValues, option => extendedProperty.AddToListOfValues(convertToTypeFunc(option)));
         return extendedProperty;
      }

      public override ModelExtendedProperty MapToModel(SnapshotExtendedProperty snapshot)
      {

         ModelExtendedProperty property;

         if (snapshot.Type == typeof(string))
         {
            property = addOptionsToExtendedProperty(snapshot, option => option.ToString());
         }
         else if (snapshot.Type == typeof(double))
         {
            property = addOptionsToExtendedProperty(snapshot, option => double.Parse(option.ToString()));
         }
         else if (snapshot.Type == typeof(bool))
         {
            property = addOptionsToExtendedProperty(snapshot, option => bool.Parse(option.ToString()));
         }
         else
         {
            property = addOptionsToExtendedProperty(snapshot, option => option);
         }

         property.Description = snapshot.Description;
         property.ReadOnly = snapshot.ReadOnly;
         property.FullName = snapshot.FullName;
         property.Name = snapshot.Name;
         property.ValueAsObject = snapshot.Value;

         return property;
      }

      private void addOptionsToList(List<object> snapshotListOfValues, Action<object> action)
      {
         snapshotListOfValues.Each(action);
      }
   }
}