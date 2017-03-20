using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ICloneSimulationTask
   {
      /// <summary>
      ///    Clone the simulation. Only a simulation for which all building blocks are uptodate can be clone
      /// </summary>
      /// <param name="simulationToClone">Simulation to clone</param>
      void Clone(Simulation simulationToClone);
   }
}