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
   public string Species => OverwriteParameterSet.GetExtendedProperty(PKSimConstants.ObjectTypes.Species);
   public string DiseaseState => OverwriteParameterSet.GetExtendedProperty(PKSimConstants.ObjectTypes.DiseaseState);
   public IReadOnlyList<OverwriteParameterValueDTO> ParameterValues { get; }
}