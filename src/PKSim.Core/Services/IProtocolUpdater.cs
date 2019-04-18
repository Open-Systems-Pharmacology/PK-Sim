using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IProtocolUpdater
   {
      void UpdateProtocol(Protocol sourceProtocol, Protocol targetProtocol);
   }
}