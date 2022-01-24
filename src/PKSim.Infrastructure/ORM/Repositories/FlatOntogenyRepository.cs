using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatOntogenyRepository : IMetaDataRepository<OntogenyMetaData>
   {
   }

   public class FlatOntogenyRepository : MetaDataRepository<OntogenyMetaData>, IFlatOntogenyRepository
   {
      public FlatOntogenyRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<OntogenyMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_ONTOGENIES)
      {
      }
   }
}