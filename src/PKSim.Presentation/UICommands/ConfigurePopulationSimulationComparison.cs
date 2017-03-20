using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ConfigurePopulationSimulationComparison : ObjectUICommand<PopulationSimulationComparison>
   {
      private readonly ISimulationComparisonCreator _simulationComparisonCreator;

      public ConfigurePopulationSimulationComparison(ISimulationComparisonCreator simulationComparisonCreator)
      {
         _simulationComparisonCreator = simulationComparisonCreator;
      }

      protected override void PerformExecute()
      {
         _simulationComparisonCreator.ConfigurePopulationSimulationComparison(Subject);
      }
   }
}