using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ITransportDirectionRepository : IRepository<TransportDirection>
   {
      TransportDirection ById(TransportDirectionId transportDirectionId);
   }
}