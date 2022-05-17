using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterValueRepository : IMetaDataRepository<ParameterValueMetaData>
   {
   }

   public class FlatParameterValueRepository : MetaDataRepository<ParameterValueMetaData>, IFlatParameterValueRepository
   {
      public FlatParameterValueRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterValueMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PARAMETER_VALUES)
      {
      }
   }

   public class ParameterValueRepository : ParameterMetaDataRepository<ParameterValueMetaData>, IParameterValueRepository
   {
      public ParameterValueRepository(
         IFlatParameterValueRepository flatParameterValueRepo,
         IFlatContainerRepository flatContainerRepo,
         IValueOriginRepository valueOriginRepository,
         IFlatContainerParameterDescriptorConditionRepository flatContainerParameterDescriptorConditionRepository,
         ICriteriaConditionToDescriptorConditionMapper descriptorConditionMapper
      ) :
         base(flatParameterValueRepo, flatContainerRepo, valueOriginRepository, flatContainerParameterDescriptorConditionRepository, descriptorConditionMapper)
      {
      }
   }
}