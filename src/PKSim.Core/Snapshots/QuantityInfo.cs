using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class QuantityInfo : SnapshotBase
   {
      public List<string> Path { set; get; }
      public string Type { set; get; }
      public int OrderIndex { set; get; }
   }
}