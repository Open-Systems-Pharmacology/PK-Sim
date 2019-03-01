using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison
{
   public class AlternativeDiffBuilder : DiffBuilder<ParameterAlternative>
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      public AlternativeDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<ParameterAlternative> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareStringValues(x => x.IsDefault.ToString(), PKSimConstants.UI.IsDefault, comparison);
      }
   }
}