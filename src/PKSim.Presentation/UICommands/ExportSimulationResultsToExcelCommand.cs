using OSPSuite.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ExportSimulationResultsToExcelCommand : ActiveObjectUICommand<IndividualSimulation>
   {
      private readonly ISimulationExportTask _simulationTask;

      public ExportSimulationResultsToExcelCommand(ISimulationExportTask simulationTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationTask = simulationTask;
      }

      protected override async void PerformExecute()
      {
         await _simulationTask.SecureAwait(x=>x.ExportResultsToExcel(Subject));
      }
   }
}