using System.Collections.Generic;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class UnknownDistributionFormula : DistributionFormula
   {
      public UnknownDistributionFormula() : base(DistributionType.Unknown)
      {
      }

      protected override double CalculateFor(IReadOnlyList<IObjectReference> usedObjects, IUsingFormula dependentObject)
      {
         return double.NaN;
      }

      public override double CalculatePercentileForValue(double value, IUsingFormula refObject)
      {
         return double.NaN;
      }

      public override double CalculateValueFromPercentile(double percentile, IUsingFormula refObject)
      {
         return double.NaN;
      }

      public override double ProbabilityDensityFor(double value, IUsingFormula refObject)
      {
         return double.NaN;
      }

      public override double RandomDeviate(RandomGenerator randomGenerator, IUsingFormula refObject, double min, double max)
      {
         return double.NaN;
      }

      public override double RandomDeviate(RandomGenerator randomGenerator, IUsingFormula refObject)
      {
         return double.NaN;
      }
   }
}