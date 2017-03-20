using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class RandomPopulationSettingsDiffBuilder : DiffBuilder<RandomPopulationSettings>
   {
      private readonly EnumerableComparer _enumerableComparer;

      public RandomPopulationSettingsDiffBuilder(EnumerableComparer enumerableComparer)
      {
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<RandomPopulationSettings> comparison)
      {
         CompareValues(x => x.BaseIndividual.Name, PKSimConstants.UI.BasedOnIndividual, comparison);
         CompareValues(x => x.NumberOfIndividuals, PKSimConstants.UI.NumberOfIndividuals, comparison);
         _enumerableComparer.CompareEnumerables(comparison, x => x.GenderRatios, x => x.Gender);
         _enumerableComparer.CompareEnumerables(comparison, x => x.ParameterRanges, x => x.ParameterDisplayName);
      }
   }
}