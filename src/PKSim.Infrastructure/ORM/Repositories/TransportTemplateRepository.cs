using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class TransportTemplateRepository : MetaDataRepository<TransportTemplate>, ITransportTemplateRepository
   {
      public TransportTemplateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<TransportTemplate> mapper) :
         base(dbGateway, mapper, CoreConstants.ORM.ViewTransports)
      {
      }
   }
}