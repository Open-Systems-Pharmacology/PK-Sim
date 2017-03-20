using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class ImportPopulationSettingsDiffBuilder : DiffBuilder<ImportPopulationSettings>
   {
      private readonly EnumerableComparer _enumerableComparer;

      public ImportPopulationSettingsDiffBuilder(EnumerableComparer enumerableComparer)
      {
         _enumerableComparer = enumerableComparer;
      }

      public override void Compare(IComparison<ImportPopulationSettings> comparison)
      {
         _enumerableComparer.CompareEnumerables(comparison, x => x.AllFiles, x => x.FilePath);
      }
   }
}