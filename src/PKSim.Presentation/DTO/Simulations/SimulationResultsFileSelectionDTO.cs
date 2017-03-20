using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationResultsFileSelectionDTO : ImportFileSelectionDTO
   {
      private int? _numberOfIndividuals;
      private int? _numberOfQuantities;

      public int? NumberOfIndividuals
      {
         get { return _numberOfIndividuals; }
         set
         {
            _numberOfIndividuals = value;
            OnPropertyChanged(() => NumberOfIndividuals);
         }
      }

      public int? NumberOfQuantities
      {
         get { return _numberOfQuantities; }
         set
         {
            _numberOfQuantities = value;
            OnPropertyChanged(() => NumberOfQuantities);
         }
      }

      public static SimulationResultsFileSelectionDTO From(SimulationResultsFile simulationResultsFile)
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