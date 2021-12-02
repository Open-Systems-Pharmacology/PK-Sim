using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatSchemaItemRepository : IMetaDataRepository<FlatSchemaItem>
   {
   }

   public class FlatSchemaItemRepository : MetaDataRepository<FlatSchemaItem>, IFlatSchemaItemRepository
   {
      public FlatSchemaItemRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatSchemaItem> mapper) :
         base(dbGateway, mapper, CoreConstants.ORM.VIEW_SCHEMA_ITEMS)
      {
      }
   }
}