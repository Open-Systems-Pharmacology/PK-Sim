using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class IndividualDiffBuilder : DiffBuilder<Individual>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;
      private readonly EnumerableComparer _enumerableComparer;

      public IndividualDiffBuilder(
         ContainerDiffBuilder containerDiffBuilder,
         IObjectComparer comparer,
         EnumerableComparer enumerableComparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<Individual> comparison)
      {
         _comparer.Compare(comparison.ChildComparison(x => x.OriginData));
         _enumerableComparer.CompareEnumerables(comparison, x => x.AllExpressionProfiles().AllNames(), x => x, missingItemType: PKSimConstants.ObjectTypes.ExpressionProfile);
         _containerDiffBuilder.Compare(comparison);
      }
   }
}