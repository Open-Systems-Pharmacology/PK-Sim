using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots
{
   public class SimulationComparison : SnapshotBase
   {
      public string[] Simulations { get; set; }
      public CurveChart IndividualComparison { get; set; }
      public PopulationAnalysisChart[] PopulationComparisons { get; set; }
      public GroupingItem ReferenceGroupingItem { get; set; }
      public string ReferenceSimulation { get; set; }
   }
}