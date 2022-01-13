using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterRHSRepository : IMetaDataRepository<FlatParameterRHS>
   {
   }

   public class FlatParameterRHSRepository : MetaDataRepository<FlatParameterRHS>, IFlatParameterRHSRepository
   {
      public FlatParameterRHSRepository(IDbGateway dbGateway,
                                    IDataTableToMetaDataMapper<FlatParameterRHS> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PARAMETER_RHS)
      {
      }
   }

}
