using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Curve : IWithName
   {
      public string Name { get; set; }
      public string X { get; set; }
      public string Y { get; set; }
      public CurveOptions CurveOptions { get; set; }
   }
}