using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Chart
{
   public class SimulationTimeProfileChart : ChartWithObservedData, ISimulationAnalysis
   {
      public IAnalysable Analysable { get; set; }
   }
}