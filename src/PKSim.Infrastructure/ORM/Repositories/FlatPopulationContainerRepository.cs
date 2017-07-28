using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatPopulationContainerRepository : IMetaDataRepository<FlatPopulationContainer>
   {
      IEnumerable<FlatPopulationContainer> AllSubContainerFor(string populationName, int parentContainerId);
   }

   public class FlatPopulationContainerRepository : MetaDataRepository<FlatPopulationContainer>, IFlatPopulationContainerRepository
   {
      private readonly Cache<string, Cache<int, IEnumerable<FlatPopulationContainer>>> _allContainerCache = new Cache<string, Cache<int, IEnumerable<FlatPopulationContainer>>>();

      public FlatPopulationContainerRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatPopulationContainer> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewPopulationContainers)
      {
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var flatSpeciesContainers in AllElements().GroupBy(x => x.Population))
         {
            _allContainerCache.Add(flatSpeciesContainers.Key, cacheFor(flatSpeciesContainers));
         }
      }

      private Cache<int, IEnumerable<FlatPopulationContainer>> cacheFor(IEnumerable<FlatPopulationContainer> flatSpeciesContainers)
      {
         var cache = new Cache<int, IEnumerable<FlatPopulationContainer>>(x => Enumerable.Empty<FlatPopulationContainer>());

         foreach (var flatSpeciesContainer in flatSpeciesContainers.GroupBy(x => x.ParentId))
         {
            cache.Add(flatSpeciesContainer.Key, flatSpeciesContainer);
         }
         return cache;
      }

      public IEnumerable<FlatPopulationContainer> AllSubContainerFor(string populationName, int parentContainerId)
      {
         Start();
         return _allContainerCache[populationName][parentContainerId];
      }
   }
}