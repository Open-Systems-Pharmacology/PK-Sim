using System.Drawing;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Snapshots
{
   public class CurveOptions
   {
      public AxisTypes? yAxisType;
      public bool? Visible { get; set; }
      public bool? ShouldShowLLOQ { get; set; }
      public bool? VisibleInLegend { get; set; }
      public InterpolationModes? InterpolationMode { get; set; }
      public Color? Color { get; set; }
      public int? LegendIndex { get; set; }
      public LineStyles? LineStyle { get; set; }
      public Symbols? Symbol { get; set; }
   }
}