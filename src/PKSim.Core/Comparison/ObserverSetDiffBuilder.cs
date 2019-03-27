using OSPSuite.Core.Comparison;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class ObserverSetDiffBuilder : DiffBuilder<ObserverSet>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public ObserverSetDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<ObserverSet> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
      }
   }
}