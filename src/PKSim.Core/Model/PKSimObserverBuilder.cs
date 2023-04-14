using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public interface IPKSimObserverBuilder : IWithFormula
   {
      ObserverBuilder ObserverBuilder { get; }
   }

   public class PKSimObserverBuilder : IPKSimObserverBuilder
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public ObserverBuilder ObserverBuilder { get; }

      public PKSimObserverBuilder(ObserverBuilder observerBuilder)
      {
         ObserverBuilder = observerBuilder;
      }
   }
}