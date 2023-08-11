using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository : 
      IMetaDataRepository<FlatIndividualParametersSameFormulaOrValueForAllSpecies>
   {
   }

   public class FlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository : 
      MetaDataRepository<FlatIndividualParametersSameFormulaOrValueForAllSpecies>, 
      IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository
   {
      public FlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatIndividualParametersSameFormulaOrValueForAllSpecies> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES)
      {
      }
   }
}
