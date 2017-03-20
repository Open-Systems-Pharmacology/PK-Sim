using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class PKSimTransport : TransportBuilder, IPKSimProcess
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourcePassiveTransport = sourceObject as PKSimTransport;
         if (sourcePassiveTransport == null) return;
         CalculationMethod = sourcePassiveTransport.CalculationMethod;
         Rate = sourcePassiveTransport.Rate;
      }
   }
}