using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatOldCalculationMethodNameMappingRepository : IMetaDataRepository<FlatOldNameMapping>
   {
   }

   public class FlatOldCalculationMethodNameMappingRepository : MetaDataRepository<FlatOldNameMapping>, IFlatOldCalculationMethodNameMappingRepository
   {
      public FlatOldCalculationMethodNameMappingRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatOldNameMapping> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewOldCalculationMethodNameMappings)
      {
      }
   }

}
