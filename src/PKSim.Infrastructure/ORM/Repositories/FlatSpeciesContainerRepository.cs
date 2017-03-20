using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatSpeciesContainerRepository : IMetaDataRepository<FlatSpeciesContainer>
   {
      IEnumerable<FlatSpeciesContainer> AllSubContainer(string species, int parentContainerId);
   }

   public class FlatSpeciesContainerRepository : MetaDataRepository<FlatSpeciesContainer>, IFlatSpeciesContainerRepository
   {
      private readonly ICache<string, ICache<int, IEnumerable<FlatSpeciesContainer>>> _allContainerCache = new Cache<string, ICache<int, IEnumerable<FlatSpeciesContainer>>>();

      public FlatSpeciesContainerRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatSpeciesContainer> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewSpeciesContainers)
      {
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var flatSpeciesContainers in AllElements().GroupBy(x => x.Species))
         {
            _allContainerCache.Add(flatSpeciesContainers.Key, cacheFor(flatSpeciesContainers));
         }
      }

      private ICache<int, IEnumerable<FlatSpeciesContainer>> cacheFor(IEnumerable<FlatSpeciesContainer> flatSpeciesContainers)
      {
         var cache = new Cache<int, IEnumerable<FlatSpeciesContainer>>(x => Enumerable.Empty<FlatSpeciesContainer>());

         foreach (var flatSpeciesContainer in flatSpeciesContainers.GroupBy(x => x.ParentId))
         {
            cache.Add(flatSpeciesContainer.Key, flatSpeciesContainer);
         }
         return cache;
      }

      public IEnumerable<FlatSpeciesContainer> AllSubContainer(string species, int parentContainerId)
      {
         Start();
         var speciesCache = _allContainerCache[species];
         return speciesCache[parentContainerId];
      }
   }
}