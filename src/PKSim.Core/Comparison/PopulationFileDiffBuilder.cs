using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public class PopulationFileDiffBuilder : DiffBuilder<PopulationFile>
   {
      public override void Compare(IComparison<PopulationFile> comparison)
      {
         CompareStringValues(x => x.FilePath, PKSimConstants.UI.PopulationFilePath, comparison);
         CompareValues(x => x.NumberOfIndividuals, PKSimConstants.UI.NumberOfIndividuals, comparison);
      }
   }
}