using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatTransporterContainerTemplateRepository : IMetaDataRepository<FlatTransporterContainerTemplate>
   {
   }

   public class FlatTransporterContainerTemplateRepository : MetaDataRepository<FlatTransporterContainerTemplate>, IFlatTransporterContainerTemplateRepository
   {
      public FlatTransporterContainerTemplateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatTransporterContainerTemplate> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewIndividualActiveTransports)
      {
      }
   }
}