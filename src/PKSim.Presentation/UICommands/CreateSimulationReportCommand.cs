using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class CreateSimulationReportCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly ISimulationExportTask _simulationExportTask;

      public CreateSimulationReportCommand(ISimulationExportTask simulationExportTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationExportTask = simulationExportTask;
      }

      protected override void PerformExecute()
      {
         _simulationExportTask.CreateSimulationReport(Subject);
      }
   }
}