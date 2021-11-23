using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class ExpressionProfileDiffBuilder : DiffBuilder<ExpressionProfile>
   {
      private readonly IObjectComparer _comparer;

      public ExpressionProfileDiffBuilder(IObjectComparer comparer)
      {
         _comparer = comparer;
      }

      public override void Compare(IComparison<ExpressionProfile> comparison)
      {
         CompareValues(x => x.Category, PKSimConstants.UI.ExpressionProfileCategory, comparison);
         CompareValues(x => x.Species, PKSimConstants.UI.Species, comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.Individual));
      }
   }
}