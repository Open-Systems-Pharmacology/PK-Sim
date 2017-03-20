using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class RandomPopulationDiffBuilder : DiffBuilder<RandomPopulation>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public RandomPopulationDiffBuilder(ContainerDiffBuilder containerDiffBuilder,IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<RandomPopulation> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.Settings));
      }
   }
}