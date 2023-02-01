using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{

   public interface IFlatTransporterTemplateRepository : IMetaDataRepository<FlatTransporterTemplate>
   {
   }

   public class FlatTransporterTemplateRepository : MetaDataRepository<FlatTransporterTemplate>, IFlatTransporterTemplateRepository
   {
      public FlatTransporterTemplateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatTransporterTemplate> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_KNOWN_TRANSPORTERS)
      {
      }
   }
}