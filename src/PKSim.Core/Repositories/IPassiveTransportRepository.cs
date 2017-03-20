using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IPassiveTransportRepository : IStartableRepository<PKSimTransport>
   {
      IEnumerable<PKSimTransport> AllFor(ModelProperties modelProperties);
   }
}