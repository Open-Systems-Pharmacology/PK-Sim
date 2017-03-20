using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class UnknownDistributionFormulaSpecificationFactory : IDistributionFormulaSpecificationFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;

      public UnknownDistributionFormulaSpecificationFactory(IObjectBaseFactory objectBaseFactory)
      {
         _objectBaseFactory = objectBaseFactory;
      }

      public IDistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData)
      {
         throw new PKSimException("Should not be called");
      }

      public IDistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter)
      {
         return _objectBaseFactory.Create<UnknownDistributionFormula>();
      }

      public void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData)
      {
         throw new PKSimException("Should not be called");
      }

      public bool IsSatisfiedBy(IDistributionMetaData distribution)
      {
         return distribution.Distribution == DistributionTypes.Unknown;
      }

      public bool IsSatisfiedBy(IEnumerable<ParameterDistributionMetaData> distributions)
      {
         return distributions.All(IsSatisfiedBy);
      }
   }
}