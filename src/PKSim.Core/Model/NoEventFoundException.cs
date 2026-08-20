using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class NoEventFoundException : PKSimException
   {
      //event placeholder of a protocol that could not be resolved to an event building block
      public NoEventFoundException(Protocol protocol, string eventKey)
         : base(PKSimConstants.Error.NoEventFoundForPlaceholder(protocol.Name, eventKey))
      {
      }

      //standalone simulation event mapping that could not be resolved to an event building block
      public NoEventFoundException(string templateEventId)
         : base(PKSimConstants.Error.NoEventFoundForSimulationEvent(templateEventId))
      {
      }
   }
}
