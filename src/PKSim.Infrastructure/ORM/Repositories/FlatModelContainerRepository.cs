using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelContainerRepository : IMetaDataRepository<FlatModelContainer>
   {
      IEnumerable<FlatModelContainer> AllSubContainerFor(string modelName, int parentContainerId);
   }

   public class FlatModelContainerRepository : MetaDataRepository<FlatModelContainer>, IFlatModelContainerRepository
   {
      private readonly ICache<string, ICache<int, IEnumerable<FlatModelContainer>>> _allContainerCache = new Cache<string, ICache<int, IEnumerable<FlatModelContainer>>>();

      public FlatModelContainerRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatModelContainer> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewModelContainers)
      {
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var flatModelContainer in AllElements().GroupBy(x => x.Model))
         {
            _allContainerCache.Add(flatModelContainer.Key, cacheFor(flatModelContainer));
         }
      }

      public IEnumerable<FlatModelContainer> AllSubContainerFor(string modelName, int parentContainerId)
      {
         Start();
         var modelCache = _allContainerCache[modelName];
         return modelCache[parentContainerId];
      }

      private ICache<int, IEnumerable<FlatModelContainer>> cacheFor(IEnumerable<FlatModelContainer> flatModelContainers)
      {
         var cache = new Cache<int, IEnumerable<FlatModelContainer>>(x => Enumerable.Empty<FlatModelContainer>());

         foreach (var flatModelContainer in flatModelContainers.GroupBy(x => x.ParentId))
         {
            cache.Add(flatModelContainer.Key, flatModelContainer);
         }
         return cache;
      }
   }
}