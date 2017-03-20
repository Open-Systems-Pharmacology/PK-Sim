using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
    public interface IFlatPopulationGenderRepository : IMetaDataRepository<FlatPopulationGender>
    {
    }

    public class FlatPopulationGenderRepository : MetaDataRepository<FlatPopulationGender>, IFlatPopulationGenderRepository
    {
        public FlatPopulationGenderRepository(IDbGateway dbGateway,
                                          IDataTableToMetaDataMapper<FlatPopulationGender> mapper)
            : base(dbGateway, mapper, CoreConstants.ORM.ViewPopulationGenders)
        {
        }
    }
}