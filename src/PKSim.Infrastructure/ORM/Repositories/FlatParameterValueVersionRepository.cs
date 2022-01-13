using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterValueVersionRepository : IMetaDataRepository<FlatParameterValueVersion>
   {
   }

   public class FlatParameterValueVersionRepository : MetaDataRepository<FlatParameterValueVersion>, IFlatParameterValueVersionRepository
   {
      public FlatParameterValueVersionRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatParameterValueVersion> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PARAMETER_VALUE_VERSIONS)
      {
      }
   }
}