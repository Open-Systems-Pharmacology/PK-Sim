using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatOldParameterNameMappingRepository : IMetaDataRepository<FlatOldNameMapping>
   {
   }

   public class FlatOldParameterNameMappingRepository : MetaDataRepository<FlatOldNameMapping>, IFlatOldParameterNameMappingRepository
   {
      public FlatOldParameterNameMappingRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatOldNameMapping> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewOldParameterNameMappings)
      {
      }
   }

}
