using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLISimulationChartsLoader : ISimulationChartsLoader
   {
      public void LoadChartsFor(Simulation simulation)
      {
         // Intentionally no-op in headless CLI/R: chart loading is a Presentation-side
         // concern (the real impl injects IChartTask from PKSim.Presentation). LazyLoadTask
         // injects ISimulationChartsLoader unconditionally, so this stub satisfies IoC
         // without dragging Presentation into the cross-platform R/CLI graph.
      }
   }
}
