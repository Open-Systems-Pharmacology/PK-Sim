using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ISimulationSettingsRetriever
   {
      /// <summary>
      ///    Starts the use case to define settings for the simulation
      /// </summary>
      /// <param name="simulation">The simulation for which settings should be defined</param>
      /// <returns>The updated settings or null if user cancels</returns>
      OutputSelections SettingsFor(Simulation simulation);

      /// <summary>
      ///    Makes sure that the settings used do match the simulation. They could have become out of sync after a clone or configuration
      ///    where the compound was changed
      /// </summary>
      void SynchronizeSettingsIn(Simulation simulation);

      void CreatePKSimDefaults(Simulation simulation);
   }
}