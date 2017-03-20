using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{

   public interface IFlatObserverDescriptorConditionRepository : IMetaDataRepository<FlatObserverDescriptorCondition>
   {
   }

   public class FlatObserverDescriptorConditionRepository : MetaDataRepository<FlatObserverDescriptorCondition>, IFlatObserverDescriptorConditionRepository
   {
      public FlatObserverDescriptorConditionRepository(IDbGateway dbGateway,
                                                      IDataTableToMetaDataMapper<FlatObserverDescriptorCondition> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewObserverDescriptorConditions)
      {
      }
   }
}
