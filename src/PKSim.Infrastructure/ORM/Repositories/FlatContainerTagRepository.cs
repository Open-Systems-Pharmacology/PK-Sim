using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatContainerTagRepository : IMetaDataRepository<FlatContainerTag>
   {
   }

   public class FlatContainerTagRepository : MetaDataRepository<FlatContainerTag>, IFlatContainerTagRepository
   {
      public FlatContainerTagRepository(IDbGateway dbGateway,
                                        IDataTableToMetaDataMapper<FlatContainerTag> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CONTAINER_TAGS)
      {
      }
   }

}
