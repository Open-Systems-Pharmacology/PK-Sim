using PKSim.Core.Model;
using PKSim.Presentation.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ImportSimulationResultsCommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly ISimulationTask _simulationTask;

      public ImportSimulationResultsCommand(ISimulationTask simulationTask, IActiveSubjectRetriever activeSubjectRetriever):base(activeSubjectRetriever)
      {
         _simulationTask = simulationTask;
      }

      protected override void PerformExecute()
      {
         _simulationTask.ImportResultsIn(Subject);
      }
   }
}