using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class OverwriteParameterSet : SnapshotBase
   {
      public bool? IsDefault { get; set; }
      public ExtendedProperty[] ExtendedProperties { get; set; }
      public ParameterValue[] ParameterValues { get; set; }
   }
}
