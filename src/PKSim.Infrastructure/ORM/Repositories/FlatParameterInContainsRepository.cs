using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterInContainsRepository : IMetaDataRepository<ParameterMetaData>
   {
   }

   public class FlatParameterInContainsRepository : MetaDataRepository<ParameterMetaData>, IFlatParameterInContainsRepository
   {
      public FlatParameterInContainsRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewParametersInContainers)
      {
      }
   }
}