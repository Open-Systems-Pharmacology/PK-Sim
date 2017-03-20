using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO
{
   public class PopulationSimulationSelectionDTO : ObjectSelectionDTO<PopulationSimulation>
   {
      public PopulationSimulationSelectionDTO(PopulationSimulation simulation) : base(simulation)
      {
      }

      public PopulationSimulation Simulation
      {
         get { return Object; }
      }
   }
}