using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class DataColumn : SnapshotBase
   {
      public DataColumn[] RelatedColumns { get; set; }
      public QuantityInfo QuantityInfo { get; set; }
      public DataInfo DataInfo { get; set; }
      public List<float> Values { get; set; }
      public string Dimension { get; set; }
      public string Unit { get; set; }
   }
}