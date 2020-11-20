using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class TransporterExpressionContainerDiffBuilder : DiffBuilder<TransporterExpressionContainer>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public TransporterExpressionContainerDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<TransporterExpressionContainer> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareValues(x => x.TransportDirection, PKSimConstants.UI.TransportDirection, comparison);
      }
   }
}