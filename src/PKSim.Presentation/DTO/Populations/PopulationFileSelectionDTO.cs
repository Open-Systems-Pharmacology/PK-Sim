using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Populations
{
   public class PopulationFileSelectionDTO : ImportFileSelectionDTO
   {
      private int? _count;

      public override string FilePath
      {
         set
         {
            _count = null;
            base.FilePath = value;
         }
      }

      public int? Count
      {
         get { return _count; }
         set
         {
            _count = value;
            OnPropertyChanged(() => Count);
         }
      }

      public static PopulationFileSelectionDTO From(PopulationFile populationFile)
      {
         return new PopulationFileSelectionDTO
            {
               FilePath = populationFile.FilePath,
               Status = populationFile.Status,
               Count = populationFile.NumberOfIndividuals,
               Messages = populationFile.Log
            };
      }
   }
}