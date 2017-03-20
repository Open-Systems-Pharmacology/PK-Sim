using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationUpdaterAfterDeserialization
   {
      void UpdateSimulation(Simulation simulation);
   }
}