using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatApplicationRepository : IMetaDataRepository<FlatApplication>
   {
   }

   public class FlatApplicationRepository : MetaDataRepository<FlatApplication>, IFlatApplicationRepository
   {
      public FlatApplicationRepository(IDbGateway dbGateway,
                                       IDataTableToMetaDataMapper<FlatApplication> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewApplications)
      {
      }
   }
}
