using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationEnzymaticProcessSelectionDTO : SimulationPartialProcessSelectionDTO
   {
      public string MetaboliteName { get; set; }
      
      public SimulationEnzymaticProcessSelectionDTO(SimulationPartialProcess enzymaticProcess)
         : base(enzymaticProcess)
      {
      }

   }
}