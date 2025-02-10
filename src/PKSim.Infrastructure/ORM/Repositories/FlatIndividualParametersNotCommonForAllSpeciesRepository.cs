using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatIndividualParametersNotCommonForAllSpeciesRepository : IMetaDataRepository<FlatIndividualParametersNotCommonForAllSpecies>
   {
   }

   public class FlatIndividualParametersNotCommonForAllSpeciesRepository : MetaDataRepository<FlatIndividualParametersNotCommonForAllSpecies>, IFlatIndividualParametersNotCommonForAllSpeciesRepository
   {
      public FlatIndividualParametersNotCommonForAllSpeciesRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatIndividualParametersNotCommonForAllSpecies> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_INDIVIDUAL_PARAMETER_NOT_FOR_ALL_SPECIES)
      {
      }
   }
}