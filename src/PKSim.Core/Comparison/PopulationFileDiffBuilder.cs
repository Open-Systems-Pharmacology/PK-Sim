using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class PopulationFileDiffBuilder : DiffBuilder<PopulationImportFile>
   {
      public override void Compare(IComparison<PopulationImportFile> comparison)
      {
         CompareStringValues(x => x.FilePath, PKSimConstants.UI.PopulationFilePath, comparison);
         CompareValues(x => x.NumberOfIndividuals, PKSimConstants.UI.NumberOfIndividuals, comparison);
      }
   }
}