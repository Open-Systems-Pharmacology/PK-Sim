using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
    public interface IFlatModelSpeciesRepository : IMetaDataRepository<FlatModelSpecies>
    {
    }

    public class FlatModelSpeciesRepository : MetaDataRepository<FlatModelSpecies>, IFlatModelSpeciesRepository
    {
        public FlatModelSpeciesRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatModelSpecies> mapper)
            : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MODEL_SPECIES)
        {
        }
    }
}
