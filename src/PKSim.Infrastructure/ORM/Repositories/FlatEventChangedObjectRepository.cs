using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{

   public interface IFlatEventChangedObjectRepository : IMetaDataRepository<FlatEventChangedObject>
   {
      IEnumerable<FlatEventChangedObject> ChangedObjectsFor(int eventId);
   }

   public class FlatEventChangedObjectRepository : MetaDataRepository<FlatEventChangedObject>, IFlatEventChangedObjectRepository
   {
      public FlatEventChangedObjectRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatEventChangedObject> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_EVENT_CHANGED_OBJECTS)
      {
      }

      public IEnumerable<FlatEventChangedObject> ChangedObjectsFor(int eventId)
      {
         return All().Where(co => co.Id == eventId);
      }
   }

}
