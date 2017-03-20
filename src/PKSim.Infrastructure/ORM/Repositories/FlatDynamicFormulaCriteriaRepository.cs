using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatDynamicFormulaCriteriaRepository : IMetaDataRepository<FlatCalculationMethodRateDescriptorConditions>
   {
   }

   public class FlatDynamicFormulaCriteriaRepository : MetaDataRepository<FlatCalculationMethodRateDescriptorConditions>, IFlatDynamicFormulaCriteriaRepository
   {
      public FlatDynamicFormulaCriteriaRepository(IDbGateway dbGateway,
                                       IDataTableToMetaDataMapper<FlatCalculationMethodRateDescriptorConditions> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewDynamicFormulaCriteriaRepository)
      {
      }
   }

}
