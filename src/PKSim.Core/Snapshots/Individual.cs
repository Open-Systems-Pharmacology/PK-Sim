namespace PKSim.Core.Snapshots
{
   public class Individual : SnapshotBase
   {
      public int? Seed { get; set; }

      public OriginData OriginData { get; set; }
      public LocalizedParameter[] Parameters { get; set; }
      public Molecule[] Molecules { get; set; }
   }
}