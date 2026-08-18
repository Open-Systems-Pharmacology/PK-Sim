using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class OverwriteParameterSetDiffBuilder : DiffBuilder<OverwriteParameterSet>
   {
      private readonly ObjectBaseDiffBuilder _objectBaseDiffBuilder;
      private readonly IObjectComparer _comparer;
      private readonly EnumerableComparer _enumerableComparer;

      public OverwriteParameterSetDiffBuilder(ObjectBaseDiffBuilder objectBaseDiffBuilder, IObjectComparer comparer, EnumerableComparer enumerableComparer)
      {
         _objectBaseDiffBuilder = objectBaseDiffBuilder;
         _comparer = comparer;
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<OverwriteParameterSet> comparison)
      {
         _objectBaseDiffBuilder.Compare(comparison);
         CompareValues(x => x.IsDefault, PKSimConstants.UI.IsDefault, comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.ExtendedProperties));
         _enumerableComparer.CompareEnumerables(comparison, x => x.ParameterValues, x => x.Path);
      }
   }
}
