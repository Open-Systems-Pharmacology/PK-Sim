using OSPSuite.Presentation.DTO;
using PKSim.Core;

namespace PKSim.BatchTool.DTO
{
   public class InputAndOutputFolderDTO : ValidatableDTO
   {
      private readonly FolderDTO _inputFolder = new FolderDTO();
      private readonly FolderDTO _outputFolder = new FolderDTO();

      public string InputFolder
      {
         get => _inputFolder.Folder;
         set
         {
            _inputFolder.Folder = value;
            OnPropertyChanged(() => InputFolder);
         }
      }

      public string OutputFolder
      {
         get => _outputFolder.Folder;
         set
         {
            _outputFolder.Folder = value;
            OnPropertyChanged(() => OutputFolder);
         }
      }

      public InputAndOutputFolderDTO()
      {
         Rules.Add(new CompositionBusinessRule<FolderDTO, string>(_inputFolder, x => x.Folder, nameof(InputFolder)));
         Rules.Add(new CompositionBusinessRule<FolderDTO, string>(_outputFolder, x => x.Folder, nameof(OutputFolder)));
      }
   }
}