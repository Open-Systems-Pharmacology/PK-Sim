using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class AdvancedParameterDiffBuilder : DiffBuilder<AdvancedParameter>
   {
      private readonly EnumerableComparer _enumerableComparer;

      public AdvancedParameterDiffBuilder(EnumerableComparer enumerableComparer)
      {
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<AdvancedParameter> comparison)
      {
         _enumerableComparer.CompareEnumerables(comparison, x => x.AllParameters, x => x.Name);
      }
   }
}