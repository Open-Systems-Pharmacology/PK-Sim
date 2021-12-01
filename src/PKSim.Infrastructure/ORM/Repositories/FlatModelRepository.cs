using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
    public interface IFlatModelRepository : IMetaDataRepository<FlatModel>
    {
    }

    public class FlatModelRepository : MetaDataRepository<FlatModel>, IFlatModelRepository
    {
        public FlatModelRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatModel> mapper)
            : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MODELS)
        {
        }
    }
}
