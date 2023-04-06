using System;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model.Extensions
{
   public static class DistributionFormulaExtensions
   {
      public static DistributionType DistributionType(this DistributionFormula formula)
      {
         if (formula.IsAnImplementationOf<NormalDistributionFormula>())
            return DistributionTypes.Normal;

         if (formula.IsAnImplementationOf<LogNormalDistributionFormula>())
            return DistributionTypes.LogNormal;

         if (formula.IsAnImplementationOf<UniformDistributionFormula>())
            return DistributionTypes.Uniform;

         if (formula.IsAnImplementationOf<DiscreteDistributionFormula>())
            return DistributionTypes.Discrete;

         if (formula.IsAnImplementationOf<UnknownDistributionFormula>())
            return DistributionTypes.Unknown;

         throw new ArgumentException(PKSimConstants.Error.DistributionUnknown(formula.ToString()));
      }
   }
}