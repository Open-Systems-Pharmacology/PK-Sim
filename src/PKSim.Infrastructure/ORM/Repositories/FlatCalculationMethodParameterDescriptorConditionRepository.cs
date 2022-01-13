using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatCalculationMethodParameterDescriptorConditionRepository : IMetaDataRepository<FlatCalculationMethodParameterDescriptorCondition>
   {
   }

   public class FlatCalculationMethodParameterDescriptorConditionRepository : MetaDataRepository<FlatCalculationMethodParameterDescriptorCondition>, IFlatCalculationMethodParameterDescriptorConditionRepository
   {
      public FlatCalculationMethodParameterDescriptorConditionRepository(IDbGateway dbGateway,
                                       IDataTableToMetaDataMapper<FlatCalculationMethodParameterDescriptorCondition> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CALCULATION_METHOD_PARAMETER_DESCRIPTOR_CONDITIONS)
      {
      }
   }

}
