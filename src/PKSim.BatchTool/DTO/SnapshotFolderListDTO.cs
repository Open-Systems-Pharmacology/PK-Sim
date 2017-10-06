using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.DTO
{
   public class SnapshotFolderListDTO : ValidatableDTO
   {
      private readonly NotifyList<FolderDTO> _folders = new NotifyList<FolderDTO>();
      private string _currentFolder;
      public SnapshotExportMode ExportMode { get; set; }
      public IReadOnlyList<FolderDTO> Folders => _folders;

      public string CurrentFolder
      {
         get => _currentFolder;
         set => SetProperty(ref _currentFolder, value);
      }

      public void AddFolder(string folder) => AddFolder(new FolderDTO {Folder = folder});

      public void AddFolder(FolderDTO folderDTO)
      {
         _folders.Add(folderDTO);
      }

      public void AddFolders(IEnumerable<string> folders)
      {
         folders?.Each(AddFolder);
      }

      public void RemoveFolder(FolderDTO folderDTO)
      {
         _folders.Remove(folderDTO);
      }

      public void ClearList()
      {
         _folders.Clear();
      }

      public bool ContainsFolder(string folder)
      {
         return _folders.Any(x => string.Equals(folder, x.Folder));
      }
   }
}