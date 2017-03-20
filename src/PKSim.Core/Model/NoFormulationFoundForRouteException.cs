using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class NoFormulationFoundForRouteException : PKSimException
   {
      public NoFormulationFoundForRouteException(Protocol protocol, ApplicationType applicationType)
         : base(PKSimConstants.Error.NoFormulationFoundForRoute(protocol.Name, applicationType.Route))
      {
      }
   }
}