using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Chart
{
   public class AxisData : IWithDisplayUnit
   {
      public IDimension Dimension { get; set; }
      public Unit DisplayUnit { get; set; }
      public Scalings Scaling { get; set; }
      public string Caption { get; set; }

      public AxisData(IDimension dimension, Unit displayUnit, Scalings scaling)
      {
         Dimension = dimension;
         DisplayUnit = displayUnit;
         Scaling = scaling;
      }
   }
}