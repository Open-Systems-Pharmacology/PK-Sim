using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Snapshots
{
   public class StatisticalAggregation
   {
      [Required]
      public string Id { get; set; }
      public LineStyles LineStyle { get; set; }
   }
}