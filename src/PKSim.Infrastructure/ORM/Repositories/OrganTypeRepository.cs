using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class OrganTypeRepository : MetaDataRepository<FlatOrganType>, IOrganTypeRepository
   {
      private readonly Cache<string, OrganType> _cache = new Cache<string, OrganType>(x => OrganType.Unknown);

      public OrganTypeRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatOrganType> mapper) :
         base(dbGateway, mapper, CoreConstants.ORM.VIEW_ORGAN_TYPES)
      {
      }

      protected override void PerformPostStartProcessing()
      {
         base.PerformPostStartProcessing();
         AllElements().Each(x => _cache.Add(x.OrganName, x.OrganType));
      }

      public OrganType OrganTypeFor(string organName)
      {
         Start();
         return string.IsNullOrEmpty(organName) ? OrganType.Unknown : _cache[organName];
      }

      public OrganType OrganTypeFor(IContainer container) => OrganTypeFor(container?.Name);
   }
}