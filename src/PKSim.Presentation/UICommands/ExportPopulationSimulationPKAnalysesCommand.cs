using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ExportPopulationSimulationPKAnalysesCommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly ISimulationExportTask _simulationTask;

      public ExportPopulationSimulationPKAnalysesCommand(ISimulationExportTask simulationTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationTask = simulationTask;
      }

      protected override void PerformExecute()
      {
         _simulationTask.SecureAwait(t => t.ExportPKAnalysesToCSVAsync(Subject));
      }
   }
}