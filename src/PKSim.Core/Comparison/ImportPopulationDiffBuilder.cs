using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class ImportPopulationDiffBuilder : DiffBuilder<ImportPopulation>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public ImportPopulationDiffBuilder(ContainerDiffBuilder containerDiffBuilder, IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<ImportPopulation> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.Settings));
      }
   }
}