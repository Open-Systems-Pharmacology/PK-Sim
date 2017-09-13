using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class DataColumn : SnapshotBase
   {
      public string Id { get; set; }
      public List<DataColumn> RelatedColumns { get; set; }
      public QuantityInfo QuantityInfo { get; set; }
      public DataInfo DataInfo { get; set; }
      public List<float> Values { get; set; }
      public string DisplayUnit { get; set; }
   }
}