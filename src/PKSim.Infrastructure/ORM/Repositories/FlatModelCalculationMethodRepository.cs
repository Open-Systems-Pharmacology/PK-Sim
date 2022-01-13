using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelCalculationMethodRepository : IMetaDataRepository<FlatModelCalculationMethod>
   {
      IEnumerable<string> ModelListFor(string calculationMethod);
      FlatModelCalculationMethod By(string model, string calculationMethodName);
   }

   public class FlatModelCalculationMethodRepository : MetaDataRepository<FlatModelCalculationMethod>, IFlatModelCalculationMethodRepository
   {

      public FlatModelCalculationMethodRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FlatModelCalculationMethod> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MODEL_CALCULATION_METHODS)
      {
      }

  
      public IEnumerable<string> ModelListFor(string calculationMethod)
      {
         return (from modelCalcMethod in All()
                 where modelCalcMethod.CalculationMethod == calculationMethod
                 select modelCalcMethod.Model).Distinct();
      }

      public FlatModelCalculationMethod By(string model, string calculationMethod)
      {
         return (from modelCalcMethod in All()
                 where modelCalcMethod.CalculationMethod == calculationMethod
                 where modelCalcMethod.Model == model
                 select modelCalcMethod).First();
      }
   }
}