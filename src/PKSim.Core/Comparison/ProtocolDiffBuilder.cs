using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class SimpleProtocolDiffBuilder : SchemaItemDiffBuilder<SimpleProtocol>
   {
      public SimpleProtocolDiffBuilder(ContainerDiffBuilder containerDiffBuilder) : base(containerDiffBuilder)
      {

      }

      public override void Compare(IComparison<SimpleProtocol> comparison)
      {
         base.Compare(comparison);
         CompareValues(x => x.DosingInterval, PKSimConstants.UI.DosingInterval, comparison);
      }
   }

   public class AdvancedProtocolDiffBuilder : DiffBuilder<AdvancedProtocol>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public AdvancedProtocolDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<AdvancedProtocol> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
      }
   }
}