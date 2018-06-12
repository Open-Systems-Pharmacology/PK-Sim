using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Comparison
{
   public static class PKSimParameterDiffBuilder
   {
      public static bool ShouldCompareParametersIn(IComparison<IParameter> comparison)
      {
         if (!comparison.ComparedObjectsDefined)
            return true;

         if (comparison.Settings.CompareHiddenEntities)
            return true;

         var parameter1 = comparison.Object1;
         var parameter2 = comparison.Object2;
         
         return parameter1.Visible && parameter2.Visible;
      }
   }
}