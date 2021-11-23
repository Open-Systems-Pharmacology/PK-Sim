using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class OriginDataDiffBuilder : DiffBuilder<OriginData>
   {
      public override void Compare(IComparison<OriginData> comparison)
      {
         CompareValues(x => x.Species, PKSimConstants.UI.Species, comparison, Equals, (o, s) => s.DisplayName);
         CompareValues(x => x.SpeciesPopulation, PKSimConstants.UI.Population, comparison, Equals, (o, p) => p.DisplayName);
         //Gender can be undefined when comparing different species
         CompareValues(x => x.Gender, PKSimConstants.UI.Gender, comparison, Equals, (o, g) => g?.DisplayName);
      }
   }
}