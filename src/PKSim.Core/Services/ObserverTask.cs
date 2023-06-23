using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverTask
   {
      IPKSimCommand AddObserver(ObserverBuilder observer, ObserverSet observerSet);
      IPKSimCommand RemoveObserver(ObserverBuilder observer, ObserverSet observerSet);

      /// <summary>
      ///    Load the observer that is defined in the file <paramref name="fileName" />
      /// </summary>
      /// <param name="fileName">Full path of the observer file</param>
      /// <returns>The observer deserialized using the file <paramref name="fileName" /></returns>
      /// <exception cref="PKSimException">is thrown if the file does not represent an observer file</exception>
      ObserverBuilder LoadObserverFrom(string fileName);
   }

   public class ObserverTask : IObserverTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IObserverLoader _observerLoader;
      private readonly IObjectIdResetter _objectIdResetter;

      public ObserverTask(IExecutionContext executionContext, IObserverLoader observerLoader, IObjectIdResetter objectIdResetter)
      {
         _executionContext = executionContext;
         _observerLoader = observerLoader;
         _objectIdResetter = objectIdResetter;
      }

      public IPKSimCommand AddObserver(ObserverBuilder observer, ObserverSet observerSet)
      {
         return new AddObserverToObserverSetCommand(observer, observerSet, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand RemoveObserver(ObserverBuilder observer, ObserverSet observerSet)
      {
         return new RemoveObserverFromObserverSetCommand(observer, observerSet, _executionContext).Run(_executionContext);
      }

      public ObserverBuilder LoadObserverFrom(string pkmlFileFullPath)
      {
         var observer = _observerLoader.Load(pkmlFileFullPath);
         _objectIdResetter.ResetIdFor(observer);
         return observer;
      }
   }
}