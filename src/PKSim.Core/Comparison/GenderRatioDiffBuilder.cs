using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class GenderRatioDiffBuilder : DiffBuilder<GenderRatio>
   {
      public override void Compare(IComparison<GenderRatio> comparison)
      {
         CompareValues(x => x.Gender, PKSimConstants.UI.Gender, comparison);
         CompareValues(x => x.Ratio, PKSimConstants.UI.GenderRatio, comparison);
      }
   }
}