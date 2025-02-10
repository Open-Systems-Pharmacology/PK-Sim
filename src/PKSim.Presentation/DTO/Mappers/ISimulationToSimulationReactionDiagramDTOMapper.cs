using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimulationToSimulationReactionDiagramDTOMapper
   {
      SimulationReactionDiagramDTO MapFrom(Simulation simulation, bool recreateDiagram);
   }
}