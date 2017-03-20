using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class ParameterRangeDiffBuilder : DiffBuilder<ParameterRange>
   {
      public override void Compare(IComparison<ParameterRange> comparison)
      {
         string parameterName = "";
         if (comparison.Object1 != null)
            parameterName = comparison.Object1.ParameterDisplayName + ": ";

         CompareStringValues(x => x.ParameterDisplayName, PKSimConstants.UI.Parameter, comparison);
         CompareValues(x => x.MinValueInDisplayUnit, parameterName + PKSimConstants.UI.MinValue, comparison);
         CompareValues(x => x.MaxValueInDisplayUnit, parameterName + PKSimConstants.UI.MaxValue, comparison);
      }
   }
}