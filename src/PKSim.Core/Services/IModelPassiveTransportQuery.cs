using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IModelPassiveTransportQuery
   {
      /// <summary>
      ///    Return the passive transports building block defined for the given simulation
      /// </summary>
      IPassiveTransportBuildingBlock AllPassiveTransportsFor(Simulation simulation);
   }
}