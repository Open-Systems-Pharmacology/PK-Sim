using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class IndividualProteinDiffBuilder : DiffBuilder<IndividualProtein>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public IndividualProteinDiffBuilder(ContainerDiffBuilder containerDiffBuilder, IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<IndividualProtein> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareValues(x => x.Localization, PKSimConstants.UI.Localization, comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.Ontogeny));
      }
   }
}