using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatCompoundProcessParameterMappingRepository : IMetaDataRepository<FlatCompoundProcessParameterMapping>
   {
   }

   public class FlatCompoundProcessParameterMappingRepository : MetaDataRepository<FlatCompoundProcessParameterMapping>, IFlatCompoundProcessParameterMappingRepository
   {
      public FlatCompoundProcessParameterMappingRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatCompoundProcessParameterMapping> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewCompoundProcessParameterMappings)
      {
      }
   }

}
