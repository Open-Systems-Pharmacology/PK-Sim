using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Exchange;

namespace PKSim.Core.Services
{
   public interface ICoreLoader
   {
      /// <summary>
      /// Load the simulation transfer that is defined in the file <paramref name="pkmlFileFullPath"/> 
      /// </summary>
      /// <param name="pkmlFileFullPath">Full path of the simulation transfer file</param>
      /// <returns>The simulation transfer deserialized using the file <paramref name="pkmlFileFullPath"/></returns>
      /// <exception cref="PKSimException">is thrown if the file does not represent a SimulationTransfer file</exception>
      SimulationTransfer LoadSimulationTransfer(string pkmlFileFullPath);

      /// <summary>
      /// Load the observer that is defined in the file <paramref name="pkmlFileFullPath"/> 
      /// </summary>
      /// <param name="pkmlFileFullPath">Full path of the observer file</param>
      /// <returns>The observer deserialized using the file <paramref name="pkmlFileFullPath"/></returns>
      /// <exception cref="PKSimException">is thrown if the file does not represent an observer file</exception>
      IObserverBuilder LoadObserver(string pkmlFileFullPath);
   }
}