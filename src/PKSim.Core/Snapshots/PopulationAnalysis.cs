namespace PKSim.Core.Snapshots
{
   public class PopulationAnalysis
   {
      public PopulationAnalysisField[] Fields { get; set; }
      public StatisticalAggregation[] Statistics { get; set; }
      public bool? ShowOutliers { get; set; }
   }
}