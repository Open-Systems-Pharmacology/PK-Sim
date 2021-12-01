using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatSpeciesCalculationMethodRepository : IMetaDataRepository<FlatSpeciesCalculationMethod>
   {
      IEnumerable<string> SpeciesListFor(string calculationMethod);
   }

   public class FlatSpeciesCalculationMethodRepository : MetaDataRepository<FlatSpeciesCalculationMethod>, IFlatSpeciesCalculationMethodRepository
   {
      public FlatSpeciesCalculationMethodRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatSpeciesCalculationMethod> mapper) :
         base(dbGateway, mapper, CoreConstants.ORM.VIEW_SPECIES_CALCULATION_METHODS)
      {
      }

      public IEnumerable<string> SpeciesListFor(string calculationMethod)
      {
         return from flatSpeciesCalcMethod in All()
                where flatSpeciesCalcMethod.CalculationMethod == calculationMethod
                select flatSpeciesCalcMethod.Species;
      }
   }

}
