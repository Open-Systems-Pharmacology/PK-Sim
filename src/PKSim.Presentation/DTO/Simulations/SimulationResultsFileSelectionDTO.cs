using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationResultsFileSelectionDTO : ImportFileSelectionDTO
   {
      private int? _numberOfIndividuals;
      private int? _numberOfQuantities;

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