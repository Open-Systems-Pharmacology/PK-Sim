using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public static class DistributionFormulaFactoryExtensions
   {
      public static void ScaleDistributionFor(this IDistributionFormulaFactory distributionFormulaFactory, IDistributedParameter currentParameter, IDistributedParameter baseParameter)
      {
         if (baseParameter == null) return;
         var factor = baseParameter.ScaleFactor();
         if (factor == 1) return;

         currentParameter.MeanParameter.Value *= factor;

         if (currentParameter.Formula.DistributionType() == DistributionTypes.Normal)
            currentParameter.DeviationParameter.Value *= factor;
      }
   }
}