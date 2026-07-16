using System.Drawing;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationResultsFileSelectionDTO : ImportFileSelectionDTO
   {
      private int? _numberOfIndividuals;
      private int? _numberOfQuantities;

      // Materializes the base-class Icon as a System.Drawing.Image for binding into the DevExpress
      // PictureEdit column. Rendering lives in OSPSuite.Presentation.Extensions.ApplicationIconExtensions.
      public Image Image => Icon.ToImage();

      public int? NumberOfIndividuals
      {
         get => _numberOfIndividuals;
         set => SetProperty(ref _numberOfIndividuals, value);
      }

      public int? NumberOfQuantities
      {
         get => _numberOfQuantities;
         set => SetProperty(ref _numberOfQuantities, value);
      }

      public static SimulationResultsFileSelectionDTO From(SimulationResultsImportFile simulationResultsFile)
      {
         return new SimulationResultsFileSelectionDTO
         {
            FilePath = simulationResultsFile.FilePath,
            Status = simulationResultsFile.Status,
            NumberOfIndividuals = simulationResultsFile.NumberOfIndividuals,
            NumberOfQuantities = simulationResultsFile.NumberOfQuantities,
            Messages = simulationResultsFile.Log
         };
      }
   }
}