using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatProcessDescriptorConditionRepository : IMetaDataRepository<FlatProcessDescriptorCondition>
   {
   }

   public class FlatProcessDescriptorConditionRepository : MetaDataRepository<FlatProcessDescriptorCondition>, IFlatProcessDescriptorConditionRepository
   {
      public FlatProcessDescriptorConditionRepository(IDbGateway dbGateway,
                                                      IDataTableToMetaDataMapper<FlatProcessDescriptorCondition> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewProcessDescriptorConditions)
      {
      }
   }
}
