using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class OverwriteParameterSetDiffBuilder : DiffBuilder<OverwriteParameterSet>
   {
      private readonly ObjectBaseDiffBuilder _objectBaseDiffBuilder;
      private readonly EnumerableComparer _enumerableComparer;

      public OverwriteParameterSetDiffBuilder(ObjectBaseDiffBuilder objectBaseDiffBuilder, EnumerableComparer enumerableComparer)
      {
         _objectBaseDiffBuilder = objectBaseDiffBuilder;
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<OverwriteParameterSet> comparison)
      {
         _objectBaseDiffBuilder.Compare(comparison);
         CompareValues(x => x.IsDefault, PKSimConstants.UI.IsDefault, comparison);
         _enumerableComparer.CompareEnumerables(comparison, x => x.ExtendedProperties, x => x.Name);
         _enumerableComparer.CompareEnumerables(comparison, x => x.ParameterValues, x => x.Path);
      }
   }
}
