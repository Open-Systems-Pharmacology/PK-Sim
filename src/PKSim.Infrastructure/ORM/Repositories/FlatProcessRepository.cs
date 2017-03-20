using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatProcessRepository : IMetaDataRepository<FlatProcess>
   {
   }

   public class FlatProcessRepository : MetaDataRepository<FlatProcess>, IFlatProcessRepository
   {
      public FlatProcessRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatProcess> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewProcesses)
      {
      }
   }
}