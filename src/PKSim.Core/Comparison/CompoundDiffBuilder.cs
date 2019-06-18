using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class CompoundDiffBuilder : DiffBuilder<Compound>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public CompoundDiffBuilder(ContainerDiffBuilder containerDiffBuilder, IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<Compound> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.CalculationMethodCache));
         CompareStringValues(x => x.IsSmallMolecule.ToString(), PKSimConstants.UI.IsSmallMolecule, comparison);
      }
   }
}