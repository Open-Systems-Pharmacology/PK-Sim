using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class OverwriteParameterSet : ObjectBase
   {
      private readonly ICache<ObjectPath, ParameterValue> _parameterValues = new Cache<ObjectPath, ParameterValue>(pv => pv.Path, x => null);

      public bool IsDefault { get; set; }

      public ExtendedProperties ExtendedProperties { get; } = new();

      public IReadOnlyList<ParameterValue> ParameterValues => _parameterValues.ToList();

      public void Add(ParameterValue parameterValue) => 
         _parameterValues[parameterValue.Path] = parameterValue;

      public void Remove(ParameterValue parameterValue) =>
         _parameterValues.Remove(parameterValue.Path);

      public void RemoveByPath(string parameterPath)
      {
         var existing = ParameterValueByPath(parameterPath);
         if (existing != null)
            Remove(existing);
      }

      /// <summary>
      /// Sets the value of a specified extended property. If the property already exists, its value is updated.
      /// If the new value is <c>null</c> or empty, the property is removed.
      /// </summary>
      /// <param name="propertyName">
      /// The name of the property to set or remove.
      /// </param>
      /// <param name="newValue">
      /// The new value to assign to the property. If <c>null</c> or empty, the property will be removed.
      /// </param>
      public void SetExtendedProperty(string propertyName, string newValue)
      {
         if (string.IsNullOrEmpty(newValue))
         {
            if (ExtendedProperties.Contains(propertyName))
               ExtendedProperties.Remove(propertyName);
         }
         else if (ExtendedProperties.Contains(propertyName))
         {
            ExtendedProperties[propertyName].ValueAsObject = newValue;
         }
         else
         {
            ExtendedProperties.Add(new ExtendedProperty<string> { Name = propertyName, Value = newValue });
         }
      }

      /// <summary>
      /// Retrieves the value of a specified extended property.
      /// </summary>
      /// <param name="propertyName">
      /// The name of the property whose value is to be retrieved.
      /// </param>
      /// <returns>
      /// The value of the specified property as a string. If the property does not exist, an empty string is returned.
      /// </returns>
      public string GetExtendedProperty(string propertyName)
      {
         return ExtendedProperties.Contains(propertyName)
            ? ExtendedProperties[propertyName].ValueAsObject?.ToString() ?? string.Empty
            : string.Empty;
      }

      public ParameterValue ParameterValueByPath(string parameterPath) =>
         _parameterValues[parameterPath.ToObjectPath()];

      /// <summary>
      ///    Replaces all path entries equal to <paramref name="oldCompoundName" /> with <paramref name="newCompoundName" />
      ///    in the paths of all parameter values.
      /// </summary>
      public void ChangeCompoundName(string oldCompoundName, string newCompoundName)
      {
         var parameterValues = _parameterValues.ToList();
         _parameterValues.Clear();
         parameterValues.Each(parameterValue =>
         {
            parameterValue.Path = new ObjectPath(parameterValue.Path.Select(entry => string.Equals(entry, oldCompoundName) ? newCompoundName : entry));
            Add(parameterValue);
         });
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);

         if (!(source is OverwriteParameterSet sourceSet))
            return;

         IsDefault = sourceSet.IsDefault;
         ExtendedProperties.UpdateFrom(sourceSet.ExtendedProperties);
         _parameterValues.Clear();
         sourceSet.ParameterValues.Each(pv => Add(cloneManager.Clone(pv)));
      }
   }
}
