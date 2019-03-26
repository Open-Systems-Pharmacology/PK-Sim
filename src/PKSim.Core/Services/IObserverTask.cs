using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverTask
   {
      IPKSimCommand AddObserver(IObserverBuilder observer, ObserverSet observerSet);
      IPKSimCommand RemoveObserver(IObserverBuilder observer, ObserverSet observerSet);


      /// <summary>
      /// Load the observer that is defined in the file <paramref name="fileName"/> 
      /// </summary>
      /// <param name="fileName">Full path of the observer file</param>
      /// <returns>The observer deserialized using the file <paramref name="fileName"/></returns>
      /// <exception cref="PKSimException">is thrown if the file does not represent an observer file</exception>
      IObserverBuilder LoadObserverFrom(string fileName);
   }
}