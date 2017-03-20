using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    This service is in charge of copying or cloning results between a souce and a target simulation.
   /// </summary>
   public interface ISimulationResultsTask
   {
      /// <summary>
      ///    Clones the results defined in the source simulation into the target simulation.
      ///    This is typically used after cloning the <paramref name="sourceSimulation"/>
      /// </summary>
      void CloneResults(Simulation sourceSimulation, Simulation targetSimulation);

      /// <summary>
      ///    Copy simulation results and charts
      ///    This is typically used after configuration the <paramref name="sourceSimulation"/>
      /// </summary>
      void CopyResults(Simulation sourceSimulation, Simulation targetSimulation);
   }
}