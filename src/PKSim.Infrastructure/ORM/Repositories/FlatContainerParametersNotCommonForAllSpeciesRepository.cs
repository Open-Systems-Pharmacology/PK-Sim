using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatContainerParametersNotCommonForAllSpeciesRepository : IMetaDataRepository<FlatContainerParametersNotCommonForAllSpecies>
   {
   }

   public class FlatContainerParametersNotCommonForAllSpeciesRepository : MetaDataRepository<FlatContainerParametersNotCommonForAllSpecies>, IFlatContainerParametersNotCommonForAllSpeciesRepository
   {
      public FlatContainerParametersNotCommonForAllSpeciesRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatContainerParametersNotCommonForAllSpecies> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CONTAINER_PARAMETER_NOT_FOR_ALL_SPECIES)
      {
      }
   }
}