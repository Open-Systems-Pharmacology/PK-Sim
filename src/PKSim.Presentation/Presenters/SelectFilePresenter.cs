using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface ISelectFilePresenter : IDisposablePresenter
   {
      FileSelection SelectFile(string caption, string filter, string defaultName, string directoryKey);
      void PerformFileSelection();
      FileSelection SelectDirectory(string caption, string directoryKey);
   }

   public class SelectFilePresenter : AbstractDisposablePresenter<ISelectFileView, ISelectFilePresenter>, ISelectFilePresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private string _defaultName;
      private string _filter;
      private string _directoryKey;
      private FileSelection _fileSelection;
      private bool _selectFile;

      public SelectFilePresenter(ISelectFileView view, IDialogCreator dialogCreator) : base(view)
      {
         _dialogCreator = dialogCreator;
      }

      public FileSelection SelectFile(string caption, string filter, string defaultName, string directoryKey)
      {
         _filter = filter;
         _defaultName = defaultName;
         return startSelection(caption, directoryKey, fileSelectionCaption: PKSimConstants.UI.FilePath, selectFile: true);
      }

      public void PerformFileSelection()
      {
         var fileFullPath = _selectFile
            ? _dialogCreator.AskForFileToSave(_view.Caption, _filter, _directoryKey, _defaultName)
            : _dialogCreator.AskForFolder(_view.Caption, _directoryKey);

         if (string.IsNullOrEmpty(fileFullPath))
            return;

         _fileSelection.FilePath = fileFullPath;
      }

      public FileSelection SelectDirectory(string caption, string directoryKey)
      {
         return startSelection(caption, directoryKey, PKSimConstants.UI.ExportDirectory, selectFile: false);
      }

      private FileSelection startSelection(string viewCaption, string directoryKey, string fileSelectionCaption, bool selectFile)
      {
         _selectFile = selectFile;
         _view.SetFileSelectionCaption(fileSelectionCaption);
         _directoryKey = directoryKey;
         _view.Caption = viewCaption;
         _fileSelection = new FileSelection();
         _view.BindTo(_fileSelection);
         _view.Display();

         return _view.Canceled ? null : _fileSelection;
      }
   }
}