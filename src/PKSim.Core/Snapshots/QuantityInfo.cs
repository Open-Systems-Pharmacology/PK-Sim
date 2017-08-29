using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class QuantityInfo : SnapshotBase
   {
      public string Path { set; get; }
      public QuantityType Type { set; get; }
      public int OrderIndex { set; get; }
   }
}