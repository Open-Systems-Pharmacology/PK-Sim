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

      public override ModelExtendedProperty MapToModel(SnapshotExtendedProperty snapshot)
      {

         ModelExtendedProperty property;

         if (snapshot.Type == typeof(string))
         {
            var stringProperty = new ExtendedProperty<string>();
            addOptionsToList(snapshot.ListOfValues, option => stringProperty.AddToListOfValues(option.ToString()));
            property = stringProperty;
         }
         else if (snapshot.Type == typeof(double))
         {
            var doubleProperty = new ExtendedProperty<double>();
            addOptionsToList(snapshot.ListOfValues, option => doubleProperty.AddToListOfValues(double.Parse(option.ToString())));
            property = doubleProperty;
         }
         else if (snapshot.Type == typeof(bool))
         {
            var booleanProperty = new ExtendedProperty<bool>();
            addOptionsToList(snapshot.ListOfValues, option => booleanProperty.AddToListOfValues(bool.Parse(option.ToString())));
            property = booleanProperty;
         }
         else
         {
            var objectProperty = new ExtendedProperty<object>();
            addOptionsToList(snapshot.ListOfValues, option => objectProperty.AddToListOfValues(option));
            property = objectProperty;
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