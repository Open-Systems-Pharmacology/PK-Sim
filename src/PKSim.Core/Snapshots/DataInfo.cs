using System;

namespace PKSim.Core.Snapshots
{
   public class DataInfo : SnapshotBase
   {
      public string Origin { get; set; }
      public string AuxiliaryType { get; set; }
      public DateTime Date { get; set; }
      public string Source { get; set; }
      public string Category { get; set; }
      public double? MolWeight { get; set; }
      public ExtendedProperties ExtendedProperties { get; set; }
      public float? LLOQ { get; set; }
      public float? ComparisonThreshold { get; set; }
   }
}