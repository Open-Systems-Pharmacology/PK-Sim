using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class IndividualDiffBuilder : DiffBuilder<Individual>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public IndividualDiffBuilder(ContainerDiffBuilder containerDiffBuilder, IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<Individual> comparison)
      {
         _comparer.Compare(comparison.ChildComparison(x => x.OriginData));
         _containerDiffBuilder.Compare(comparison);
      }
   }
}