using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotDeleteSimulationIntervalException : PKSimException
   {
      public CannotDeleteSimulationIntervalException()
         : base(PKSimConstants.Error.CannotDeleteSimulationInterval)
      {
      }
   }
}