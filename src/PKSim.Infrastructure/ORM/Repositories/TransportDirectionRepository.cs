using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class TransportDirectionRepository : MetaDataRepository<TransportDirection>, ITransportDirectionRepository
   {
      private readonly Cache<TransportDirectionId, TransportDirection> _transportDirectionCache =
         new Cache<TransportDirectionId, TransportDirection>(x => x.Id);

      public TransportDirectionRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<TransportDirection> mapper) :
         base(dbGateway, mapper, CoreConstants.ORM.VIEW_TRANSPORT_DIRECTIONS)
      {
      }


      protected override void PerformPostStartProcessing()
      {
         AllElements().Each(x => _transportDirectionCache.Add(x));
      }

      public TransportDirection ById(TransportDirectionId transportDirectionId)
      {
         Start();
         return _transportDirectionCache[transportDirectionId];
      }
   }
}