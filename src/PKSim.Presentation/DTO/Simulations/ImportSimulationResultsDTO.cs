using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ImportSimulationResultsDTO : ValidatableDTO
   {
      private string _importFolder;
      private IEnumerable<string> _messages;
      public NotifyList<SimulationResultsFileSelectionDTO> SimulationResultsFile { get; private set; }
      public NotificationType Status { get; set; }

      public IEnumerable<string> Messages
      {
         get { return _messages; }
         set
         {
            _messages = value;
            OnPropertyChanged(() => Messages);
         }
      }

      public ImportSimulationResultsDTO()
      {
         SimulationResultsFile = new NotifyList<SimulationResultsFileSelectionDTO>();
         resetMessages();
      }

      private void resetMessages()
      {
         Messages = Enumerable.Empty<string>();
      }

      public string ImportFolder
      {
         get { return _importFolder; }
         set
         {
            _importFolder = value;
            OnPropertyChanged(() => ImportFolder);
         }
      }

      public void Clear()
      {
         SimulationResultsFile.Clear();
         resetMessages();
      }

      public SimulationResultsFileSelectionDTO AddFile(string fileFullPath)
      {
         var simulationResultsFileSelectionDTO = new SimulationResultsFileSelectionDTO {FilePath = fileFullPath};
         SimulationResultsFile.Add(simulationResultsFileSelectionDTO);
         return simulationResultsFileSelectionDTO;
      }

      public SimulationResultsFileSelectionDTO AddFile(SimulationResultsFileSelectionDTO simulationResultsFileSelectionDTO)
      {
         resetMessages();
         SimulationResultsFile.Add(simulationResultsFileSelectionDTO);
         return simulationResultsFileSelectionDTO;
      }

      public void RemoveFile(SimulationResultsFileSelectionDTO fileSelectionDTO)
      {
         resetMessages();
         SimulationResultsFile.Remove(fileSelectionDTO);
      }
   }
}