using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   /// <summary>
   ///    Repository for application processes (=transports)
   ///    <para></para>
   ///    Application transports are implemented as passive transport builder
   /// </summary>
   public interface IApplicationTransportRepository : IStartableRepository<PKSimTransport>
   {
      IEnumerable<PKSimTransport> TransportsFor(string applicationName);
   }
}