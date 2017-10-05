using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots
{
   public class PopulationAnalysisChart : Chart
   {
      public PopulationAnalysisType Type { get; set; }
      public PopulationAnalysis Analysis { get; set; }
      public ObservedDataCollection ObservedDataCollection { get; set; }
      public AxisSettings XAxisSettings { get; set; }
      public AxisSettings YAxisSettings { get; set; }
      public AxisSettings[] SecondaryYAxisSettings { get; set; }
   }
}