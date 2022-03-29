using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatContainerParameterDescriptorConditionRepository : IMetaDataRepository<FlatContainerParameterDescriptorCondition>
   {
   }

   public class FlatContainerParameterDescriptorConditionRepository : MetaDataRepository<FlatContainerParameterDescriptorCondition>, IFlatContainerParameterDescriptorConditionRepository
   {
      public FlatContainerParameterDescriptorConditionRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatContainerParameterDescriptorCondition> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CONTAINER_PARAMETER_DESCRIPTOR_CONDITIONS)
      {
      }
   }
}