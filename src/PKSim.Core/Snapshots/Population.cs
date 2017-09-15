namespace PKSim.Core.Snapshots
{
   public class Population : SnapshotBase
   {
      public int Seed { get; set; }
      public PopulationSettings Settings { get; set; }
      public AdvancedParameter[] AdvancedParameters { get; set; }
   }
}