using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatNeighborhoodRepository : IMetaDataRepository<FlatNeighborhood>
   {
      FlatNeighborhood NeighborhoodFrom(int neighborhoodId);
      FlatNeighborhood NeighborhoodFrom(string neighborhoodName);
   }

   public class FlatNeighborhoodRepository : MetaDataRepository<FlatNeighborhood>, IFlatNeighborhoodRepository
   {
      private readonly ICache<int, FlatNeighborhood> _flatNeighborhoodsCachedById;
      private readonly ICache<string, FlatNeighborhood> _flatNeighborhoodsCachedByName;

      public FlatNeighborhoodRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatNeighborhood> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewNeighborhoods)
      {
         _flatNeighborhoodsCachedById = new Cache<int, FlatNeighborhood>(x => x.Id);
         _flatNeighborhoodsCachedByName = new Cache<string, FlatNeighborhood>(x => x.Name);
      }

      protected override void PerformPostStartProcessing()
      {
         _flatNeighborhoodsCachedById.AddRange(AllElements());
         _flatNeighborhoodsCachedByName.AddRange(AllElements());
      }

      public FlatNeighborhood NeighborhoodFrom(int neighborhoodId)
      {
         Start();
         return _flatNeighborhoodsCachedById[neighborhoodId];
      }

      public FlatNeighborhood NeighborhoodFrom(string neighborhoodName)
      {
         Start();
         return _flatNeighborhoodsCachedByName[neighborhoodName];
      }
   }
}