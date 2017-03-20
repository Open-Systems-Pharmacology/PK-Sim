using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatCalculationMethodParameterRateRepository : IMetaDataRepository<FlatCalculationMethodParameterRate>
   {
      /// <summary>
      ///    Returns the name of all calculation methods that can be exported as <see cref="CoreCalculationMethod" />
      /// </summary>
      IEnumerable<string> AllCalculationMethodNames { get; }
   }

   public class FlatCalculationMethodParameterRateRepository : MetaDataRepository<FlatCalculationMethodParameterRate>, IFlatCalculationMethodParameterRateRepository
   {
      public FlatCalculationMethodParameterRateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatCalculationMethodParameterRate> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewCalculationMethodParameterRates)
      {
      }

      public IEnumerable<string> AllCalculationMethodNames
      {
         get
         {
            return All()
               .Select(x => x.CalculationMethod)
               .Distinct();
         }
      }
   }
}