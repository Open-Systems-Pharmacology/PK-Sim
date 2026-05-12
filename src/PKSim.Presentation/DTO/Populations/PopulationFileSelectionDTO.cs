using System.Drawing;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using PKSim.Core.Model;

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

      // Materializes the base-class Icon as a System.Drawing.Image for binding into the DevExpress
      // PictureEdit column. Rendering lives in OSPSuite.Presentation.Extensions.ApplicationIconExtensions.
      public Image Image => Icon.ToImage();

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