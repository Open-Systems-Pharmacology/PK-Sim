namespace PKSim.Core.Snapshots
{
   public class QuantityInfo : SnapshotBase
   {
      public string Path { set; get; }

      //Cannot user QuantityType here as Snapshot validation will fail for composed Type such as "Drug, Observer"
      public string Type { set; get; }
      public int? OrderIndex { set; get; }
   }
}