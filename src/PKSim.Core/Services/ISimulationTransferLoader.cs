using OSPSuite.Core.Serialization.Exchange;

namespace PKSim.Core.Services
{
   public interface ISimulationTransferLoader
   {
      /// <summary>
      ///    Load the simulation transfer that is defined in the file <paramref name="pkmlFileFullPath" />
      /// </summary>
      /// <param name="pkmlFileFullPath">Full path of the simulation transfer file</param>
      /// <returns>The simulation transfer deserialized using the file <paramref name="pkmlFileFullPath" /></returns>
      /// <exception cref="PKSimException">is thrown if the file does not represent a SimulationTransfer file</exception>
      SimulationTransfer Load(string pkmlFileFullPath);
   }
}