using System.Drawing;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Axis
   {
      public string Unit { get; set; }
      public string Dimension { get; set; }
      public string Caption { get; set; }
      public AxisTypes Type { get; set; }
      public bool GridLines { get; set; }
      public bool Visible { get; set; }
      public float? Min { get; set; }
      public float? Max { get; set; }
      public Color DefaultColor { get; set; }
      public LineStyles DefaultLineStyle { get; set; }
      public Scalings Scaling { get; set; }
      public NumberModes NumberMode { get; set; }
   }
}