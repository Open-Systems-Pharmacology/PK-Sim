using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds;

public class OverwriteParameterSetDTO
{
   private readonly OverwriteParameterSet _overwriteParameterSet;

   public OverwriteParameterSetDTO(OverwriteParameterSet overwriteParameterSet)
   {
      _overwriteParameterSet = overwriteParameterSet;
      ParameterValues = _overwriteParameterSet.ParameterValues
         .Select(parameterValue => new OverwriteParameterValueDTO(parameterValue))
         .ToList();
   }

   public string Name => _overwriteParameterSet.Name;
   public bool IsDefault => _overwriteParameterSet.IsDefault;
   public string Species => extendedPropertyValue(PKSimConstants.UI.Species);
   public string DiseaseState => extendedPropertyValue(PKSimConstants.UI.DiseaseState);
   public IReadOnlyList<OverwriteParameterValueDTO> ParameterValues { get; }

   private string extendedPropertyValue(string propertyName)
   {
      if (!_overwriteParameterSet.ExtendedProperties.Contains(propertyName))
         return string.Empty;

      var property = _overwriteParameterSet.ExtendedProperties[propertyName];
      return property.ValueAsObject?.ToString() ?? string.Empty;
   }
}