using OSPSuite.Assets;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class RemoveUnusedContentUICommand : IUICommand
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IDeveloperTask _developerTask;

      public RemoveUnusedContentUICommand(IDialogCreator dialogCreator, IProjectRetriever projectRetriever, IDeveloperTask developerTask)
      {
         _dialogCreator = dialogCreator;
         _projectRetriever = projectRetriever;
         _developerTask = developerTask;
      }

      public void Execute()
      {
         if (string.IsNullOrEmpty(_projectRetriever.ProjectFullPath))
         {
            _dialogCreator.MessageBoxInfo("Remove unused content can only be called with a project loaded from file.");
            return;
         }

         var clear = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyClearUnusedContent, Captions.Clear, Captions.CancelButton);
         if (clear == ViewResult.No)
            return;

         var backupDone = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DidYouReallyBackupProject);
         if (backupDone == ViewResult.No)
            return;

         _developerTask.ClearUnusedContent();
      }
   }
}