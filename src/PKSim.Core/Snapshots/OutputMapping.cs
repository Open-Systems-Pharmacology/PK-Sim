using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class OutputMapping
   {
      public Scalings Scaling { get; set; }
      public float? Weight { get; set; }
      public string Path { get; set; }
      public string Simulation { get; set; }
      public string ObservedData { get; set; }
      public  float[] Weights { get; set; }
      public QuantityType? QuantityType { get; set; }
   }
}