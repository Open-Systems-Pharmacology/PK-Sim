namespace PKSim.Core.Snapshots
{
   public class Parameter : SnapshotBase
   {
      public double Value { get; set; }
      public string Unit { get; set; }
      public string ValueDescription { get; set; }
      public TableFormula TableFormula { get; set; }
   }
}