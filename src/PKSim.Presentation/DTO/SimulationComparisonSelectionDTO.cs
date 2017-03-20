using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO
{
   public class SimulationComparisonSelectionDTO
   {
      public IEnumerable<PopulationSimulationSelectionDTO> AllSimulations { get; set; }
      public PKSim.Core.Model.Simulation Reference { get; set; }
      public GroupingItemDTO GroupingItem { get; private set; }

      public SimulationComparisonSelectionDTO()
      {
         GroupingItem = new GroupingItemDTO();
         AllSimulations = new List<PopulationSimulationSelectionDTO>();
         GroupingItem.Label = PKSimConstants.UI.ReferenceSimulation;
      }
   }
}