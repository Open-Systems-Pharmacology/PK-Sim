using OSPSuite.Core.Comparison;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class ObserverSetDiffBuilder : DiffBuilder<ObserverSet>
   {
      private readonly EntityDiffBuilder _entityDiffBuilder;
      private readonly EnumerableComparer _enumerableComparer;

      public ObserverSetDiffBuilder(EntityDiffBuilder entityDiffBuilder, EnumerableComparer enumerableComparer)
      {
         _entityDiffBuilder = entityDiffBuilder;
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<ObserverSet> comparison)
      {
         _entityDiffBuilder.Compare(comparison);
         _enumerableComparer.CompareEnumerables(comparison, x => x.Observers, x => x.Name);
      }
   }
}