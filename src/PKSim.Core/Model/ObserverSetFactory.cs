using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IObserverSetFactory
   {
      ObserverSet Create();
   }

   public class ObserverSetFactory : IObserverSetFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;

      public ObserverSetFactory(IObjectBaseFactory objectBaseFactory)
      {
         _objectBaseFactory = objectBaseFactory;
      }

      public ObserverSet Create()
      {
         var observerSet = _objectBaseFactory.Create<ObserverSet>();
         observerSet.IsLoaded = true;
         return observerSet;
      }
   }
}