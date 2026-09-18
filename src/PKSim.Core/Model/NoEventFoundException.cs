using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class NoEventFoundException : PKSimException
   {
      //standalone simulation event mapping that could not be resolved to an event building block
      public NoEventFoundException(string templateEventId)
         : base(PKSimConstants.Error.NoEventFoundForSimulationEvent(templateEventId))
      {
      }
   }
}
