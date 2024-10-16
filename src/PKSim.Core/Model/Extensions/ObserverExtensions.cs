using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.Extensions
{
   public static class ObserverExtensions
   {
      public static bool IsConcentrationObserver(this Observer observer)
      {
         return string.Equals(observer.Name, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
      }

      public static bool IsConcentrationObserver(this ObserverBuilder observerBuilder)
      {
         return string.Equals(observerBuilder.Name, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
      }
   }
}