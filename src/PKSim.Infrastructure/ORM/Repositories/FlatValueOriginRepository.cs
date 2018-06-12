using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatValueOriginRepository : IMetaDataRepository<FlatValueOrigin>
   {
      FlatValueOrigin FindBy(int? id);
   }

   public class FlatValueOriginRepository : MetaDataRepository<FlatValueOrigin>, IFlatValueOriginRepository
   {
      private readonly Cache<int, FlatValueOrigin> _flatValueOriginCache = new Cache<int, FlatValueOrigin>(x => x.Id, x => null);

      public FlatValueOriginRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatValueOrigin> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewValueOrigin)
      {
      }

      public FlatValueOrigin FindBy(int? id)
      {
         Start();
         return id == null ? null : _flatValueOriginCache[id.Value];
      }

      protected override void PerformPostStartProcessing()
      {
         AllElements().Each(_flatValueOriginCache.Add);
      }
   }
}