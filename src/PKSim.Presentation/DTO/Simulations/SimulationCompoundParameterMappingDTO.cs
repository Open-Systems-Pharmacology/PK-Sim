using System.Collections.Generic;
using System.Linq;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationCompoundParameterMappingDTO
   {
      private readonly IList<CompoundParameterSelectionDTO> _allParameters;

      public SimulationCompoundParameterMappingDTO()
      {
         _allParameters = new List<CompoundParameterSelectionDTO>();
      }

      public void Add(CompoundParameterSelectionDTO compoundParameterSelectionDTO)
      {
         _allParameters.Add(compoundParameterSelectionDTO);
      }

      public IEnumerable<CompoundParameterSelectionDTO> AllParameterGroups()
      {
         return _allParameters;
      }

      public IEnumerable<PKSim.Core.Model.ParameterAlternative> SelectedAlternatives()
      {
         return _allParameters.Select(x => x.SelectedAlternative);
      }
   }
}