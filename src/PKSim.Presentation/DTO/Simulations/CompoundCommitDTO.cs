using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CompoundCommitDTO
   {
      public string CompoundName { get; init; }
      public Compound TemplateCompound { get; init; }
      public bool Selected { get; set; } = true;
      public bool CreateNew { get; set; } = true;
      public string NewSetName { get; set; }
      public OverwriteParameterSet SelectedExistingSet { get; set; }
      public IReadOnlyList<OverwriteParameterSet> AvailableExistingSets { get; init; }
      public List<ParameterCommitDTO> Parameters { get; init; } = new();
   }
}
