using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewImportIndividualSimulationCommand : IUICommand
   {
      private readonly IImportSimulationTask _importSimulationTask;
      private readonly ISimulationTask _simulationTask;
      private readonly IDialogCreator _dialogCreator;

      public NewImportIndividualSimulationCommand(IImportSimulationTask importSimulationTask, ISimulationTask simulationTask, IDialogCreator dialogCreator)
      {
         _importSimulationTask = importSimulationTask;
         _simulationTask = simulationTask;
         _dialogCreator = dialogCreator;
      }

      public void Execute()
      {
         var pkml = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectSimulationPKMLFile, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART);
         if (string.IsNullOrEmpty(pkml)) return;


         var importedSimulation = _importSimulationTask.ImportIndividualSimulation(pkml);
         if (importedSimulation == null)
            return;

         _simulationTask.AddToProject(importedSimulation, editBuildingBlock: true);
      }
   }
}