using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatEventConditionRepository : IMetaDataRepository<FlatEventCondition>
   {
      FlatEventCondition EventConditionFor(int eventId);
   }

   public class FlatEventConditionRepository : MetaDataRepository<FlatEventCondition>, IFlatEventConditionRepository
   {
      public FlatEventConditionRepository(IDbGateway dbGateway,
                                          IDataTableToMetaDataMapper<FlatEventCondition> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_EVENT_CONDITIONS)
      {
      }

      public FlatEventCondition EventConditionFor(int eventId)
      {
         return All().First(ec => ec.Id == eventId);
      }
   }
}