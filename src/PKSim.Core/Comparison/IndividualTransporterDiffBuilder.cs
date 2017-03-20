using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class IndividualTransporterDiffBuilder : DiffBuilder<IndividualTransporter>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public IndividualTransporterDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<IndividualTransporter> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareValues(x => x.TransportType, PKSimConstants.UI.TransporterType, comparison);
      }
   }
}