using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Infrastructure.Serialization.Json;

namespace PKSim.BatchTool.Presenters
{
   public interface IFolderListSnapshotPresenter : ISnapshotPresenter, IPresenter<IFolderListSnapshotView>
   {
      void AddFolder(string folder);
      void RemoveFolder(FolderDTO folderDTO);
      void ClearFolderList();
      void ImportFolderList();
      void ExportFolderList();
      void SelectFolder();
   }

   public class FolderListSnapshotPresenter : AbstractPresenter<IFolderListSnapshotView, IFolderListSnapshotPresenter>, IFolderListSnapshotPresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly SnapshotFolderListDTO _snapshotFolderListDTO;

      public FolderListSnapshotPresenter(IFolderListSnapshotView view, IDialogCreator dialogCreator) : base(view)
      {
         _dialogCreator = dialogCreator;
         _snapshotFolderListDTO = new SnapshotFolderListDTO();
         _view.BindTo(_snapshotFolderListDTO);
      }

      public SnapshotRunOptions RunOptions => new SnapshotRunOptions
      {
         ExportMode = _snapshotFolderListDTO.ExportMode,
         Folders = _snapshotFolderListDTO.Folders.Select(x => x.Folder).ToList()
      };

      public void AdjustViewHeight()
      {
         _view.AdjustHeight();
      }

      public void AddFolder(string folder)
      {
         if (string.IsNullOrEmpty(folder))
            return;

         if (_snapshotFolderListDTO.ContainsFolder(folder))
            return;
         ;

         _snapshotFolderListDTO.AddFolder(folder);
         _snapshotFolderListDTO.CurrentFolder = string.Empty;
      }

      public void RemoveFolder(FolderDTO folderDTO)
      {
         _snapshotFolderListDTO.RemoveFolder(folderDTO);
      }

      public void ClearFolderList()
      {
         _snapshotFolderListDTO.ClearList();
      }

      public void ImportFolderList()
      {
         var file = _dialogCreator.AskForFileToOpen("Open Folder List File", OSPSuite.Core.Domain.Constants.Filter.JSON_FILE_FILTER, CoreConstants.DirectoryKey.BATCH_INPUT);
         if (string.IsNullOrEmpty(file))
            return;

         ClearFolderList();
         var settings = new PKSimJsonSerializerSetings();
         var folders = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(file), settings);
         _snapshotFolderListDTO.AddFolders(folders);
      }

      public void ExportFolderList()
      {
         var file = _dialogCreator.AskForFileToSave("Save Folder List File", OSPSuite.Core.Domain.Constants.Filter.JSON_FILE_FILTER, CoreConstants.DirectoryKey.BATCH_OUTPUT);
         if (string.IsNullOrEmpty(file))
            return;

         var settings = new PKSimJsonSerializerSetings();
         File.WriteAllText(file, JsonConvert.SerializeObject(_snapshotFolderListDTO.Folders.Select(x => x.Folder), Formatting.Indented, settings));
      }

      public void SelectFolder()
      {
         var folder = _dialogCreator.AskForFolder("Browse for folder", CoreConstants.DirectoryKey.BATCH_INPUT);
         if (string.IsNullOrEmpty(folder))
            return;

         AddFolder(folder);
      }

      public override bool CanClose => base.CanClose && _snapshotFolderListDTO.Folders.Any();
   }
}