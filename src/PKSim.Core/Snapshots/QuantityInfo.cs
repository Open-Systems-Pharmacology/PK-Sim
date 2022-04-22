using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class QuantityInfo
   {
      public string Path { get; set; }
      public QuantityType? Type { get; set; }
      public int? OrderIndex { get; set; }
   }
}