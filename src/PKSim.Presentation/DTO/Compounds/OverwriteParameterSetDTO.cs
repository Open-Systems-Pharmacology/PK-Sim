using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds;

public class OverwriteParameterSetDTO
{
   public OverwriteParameterSetDTO(OverwriteParameterSet overwriteParameterSet)
   {
      OverwriteParameterSet = overwriteParameterSet;
      ParameterValues = overwriteParameterSet.ParameterValues
         .Select(parameterValue => new OverwriteParameterValueDTO(parameterValue))
         .ToList();
   }

   public OverwriteParameterSet OverwriteParameterSet { get; }
   public string Name => OverwriteParameterSet.Name;
   public bool IsDefault => OverwriteParameterSet.IsDefault;
   public string Species => extendedPropertyValue(PKSimConstants.UI.Species);
   public string DiseaseState => extendedPropertyValue(PKSimConstants.UI.DiseaseState);
   public IReadOnlyList<OverwriteParameterValueDTO> ParameterValues { get; }

   private string extendedPropertyValue(string propertyName)
   {
      if (!OverwriteParameterSet.ExtendedProperties.Contains(propertyName))
         return string.Empty;

      var property = OverwriteParameterSet.ExtendedProperties[propertyName];
      return property.ValueAsObject?.ToString() ?? string.Empty;
   }
}