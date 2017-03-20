using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class CompoundDiffBuilder : DiffBuilder<Compound>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public CompoundDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<Compound> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareStringValues(x => x.IsSmallMolecule.ToString(), PKSimConstants.UI.IsSmallMolecule, comparison);
      }
   }
}