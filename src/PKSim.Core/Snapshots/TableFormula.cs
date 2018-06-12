using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class TableFormula : SnapshotBase
   {
      public string XName { get; set; }
      public string XDimension { get; set; }
      public string XUnit { get; set; }

      public string YName { get; set; }
      public string YDimension { get; set; }
      public string YUnit { get; set; }

      public bool UseDerivedValues { get; set; }
      public List<Point> Points { get; set; }
   }

   public class DistributedTableFormula : TableFormula
   {
      public double Percentile { get; set; }
      public List<DistributionMetaData> DistributionMetaData { get; set; } = new List<DistributionMetaData>();
   }
}