using System.Collections.Generic;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class DeleteSimulationComparisonsUICommand : ObjectUICommand<IReadOnlyList<ISimulationComparison>>
   {
      private readonly ISimulationComparisonTask _simulationComparisonTask;

      public DeleteSimulationComparisonsUICommand(ISimulationComparisonTask simulationComparisonTask)
      {
         _simulationComparisonTask = simulationComparisonTask;
      }

      protected override void PerformExecute()
      {
         _simulationComparisonTask.Delete(Subject);
      }
   }
}