using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{

   public interface IFlatObserverRepository : IMetaDataRepository<FlatObserver>
   {
   }

   public class FlatObserverRepository : MetaDataRepository<FlatObserver>, IFlatObserverRepository
   {
      public FlatObserverRepository(IDbGateway dbGateway,
                                    IDataTableToMetaDataMapper<FlatObserver> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewObservers)
      {
      }
   }
}
