using System.Collections.Generic;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class UnknownDistributionFormula : DistributionFormula
   {
      protected override double CalculateFor(IEnumerable<IObjectReference> usedObjects, IUsingFormula dependentObject)
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