using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatCalculationMethodRepository : IMetaDataRepository<FlatCalculationMethod>
   {
   }

   public class FlatCalculationMethodRepository : MetaDataRepository<FlatCalculationMethod>, IFlatCalculationMethodRepository
   {
      public FlatCalculationMethodRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatCalculationMethod> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewCalculationMethods)
      {
      }
   }

}