using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public interface IPKSimObserverBuilder : IWithFormula
   {
      IObserverBuilder ObserverBuilder { get; }
   }

   public class PKSimObserverBuilder : IPKSimObserverBuilder
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public IObserverBuilder ObserverBuilder { get; private set; }

      public PKSimObserverBuilder(IObserverBuilder observerBuilder)
      {
         ObserverBuilder = observerBuilder;
      }
   }
}