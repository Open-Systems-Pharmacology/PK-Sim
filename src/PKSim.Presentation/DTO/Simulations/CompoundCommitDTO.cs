using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CompoundCommitDTO : DxValidatableDTO
   {
      public string CompoundName { get; set; }
      public Compound TemplateCompound { get; set; }
      public bool Selected { get; set; } = true;
      public bool CreateNew { get; set; } = true;
      public string NewSetName { get; set; }
      public OverwriteParameterSet SelectedExistingSet { get; set; }
      public IReadOnlyList<OverwriteParameterSet> AvailableExistingSets { get; set; }
      public List<ParameterCommitDTO> Parameters { get; set; } = new();
   }
}
