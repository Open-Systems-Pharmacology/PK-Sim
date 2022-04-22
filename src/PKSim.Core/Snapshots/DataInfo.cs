using System;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Snapshots
{
   public class DataInfo : SnapshotBase
   {
      public ColumnOrigins? Origin { get; set; }
      public AuxiliaryType AuxiliaryType { get; set; }
      public string Category { get; set; }
      public double? MolWeight { get; set; }
      public ExtendedProperty[] ExtendedProperties { get; set; }
      public float? LLOQ { get; set; }
      public float? ComparisonThreshold { get; set; }
   }
}