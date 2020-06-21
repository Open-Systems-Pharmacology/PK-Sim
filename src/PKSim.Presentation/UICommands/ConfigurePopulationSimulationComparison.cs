using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ConfigurePopulationSimulationComparison : ObjectUICommand<PopulationSimulationComparison>
   {
      private readonly ISimulationComparisonTask _simulationComparisonTask;

      public ConfigurePopulationSimulationComparison(ISimulationComparisonTask simulationComparisonTask)
      {
         _simulationComparisonTask = simulationComparisonTask;
      }

      protected override void PerformExecute()
      {
         _simulationComparisonTask.ConfigurePopulationSimulationComparison(Subject);
      }
   }
}