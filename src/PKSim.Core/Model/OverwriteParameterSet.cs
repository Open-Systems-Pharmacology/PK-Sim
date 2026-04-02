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

      public ParameterValue ParameterValueByPath(string parameterPath) =>
         _parameterValues[parameterPath.ToObjectPath()];

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
