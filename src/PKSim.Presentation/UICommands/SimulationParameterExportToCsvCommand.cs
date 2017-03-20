using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class SimulationParameterExportToCsvCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IParametersReportCreator _parametersReportCreator;
      private readonly IBuildingBlockTask _buildingBlockTask;

      public SimulationParameterExportToCsvCommand(IDialogCreator dialogCreator, IParametersReportCreator parametersReportCreator, IBuildingBlockTask buildingBlockTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _dialogCreator = dialogCreator;
         _parametersReportCreator = parametersReportCreator;
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         var fileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.SaveSimulationParameterToCsvFile, Constants.Filter.CSV_FILE_FILTER, Subject.Name);
         if (string.IsNullOrEmpty(fileName)) return;
         _buildingBlockTask.Load(Subject);

         _parametersReportCreator.ExportParametersTo(Subject.Model, fileName);
      }
   }
}