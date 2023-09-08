using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class IndividualTransporterDiffBuilder : DiffBuilder<IndividualTransporter>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;
      private readonly IObjectComparer _comparer;

      public IndividualTransporterDiffBuilder(ContainerDiffBuilder containerDiffBuilder, IObjectComparer comparer)
      {
         _containerDiffBuilder = containerDiffBuilder;
         _comparer = comparer;
      }

      public override void Compare(IComparison<IndividualTransporter> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareValues(x => x.TransportType, PKSimConstants.UI.DefaultTransporterDirection, comparison);
         _comparer.Compare(comparison.ChildComparison(x => x.Ontogeny));
      }
   }
}