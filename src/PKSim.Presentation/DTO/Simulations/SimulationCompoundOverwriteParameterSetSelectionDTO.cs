using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations;

public class SimulationCompoundOverwriteParameterSetSelectionDTO
{
   public IReadOnlyList<OverwriteParameterSet> AllOverwriteParameterSets { get; }
   public OverwriteParameterSet SelectedOverwriteParameterSet { get; set; }
   public string Label => PKSimConstants.UI.OverwriteParameterSetInCompound;

   public SimulationCompoundOverwriteParameterSetSelectionDTO(IReadOnlyList<OverwriteParameterSet> allOverwriteParameterSets, OverwriteParameterSet selectedOverwriteParameterSet)
   {
      AllOverwriteParameterSets = allOverwriteParameterSets;
      SelectedOverwriteParameterSet = selectedOverwriteParameterSet;
   }
}