using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterMetaDataRepository : IMetaDataRepository<ParameterMetaData>
   {
   }

   public class FlatParameterMetaDataRepository : MetaDataRepository<ParameterMetaData>, IFlatParameterMetaDataRepository
   {
      public FlatParameterMetaDataRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewParametersInContainers)
      {
      }
   }
}