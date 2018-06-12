using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Point
   {
      [Required]
      public double X { get; set; }

      [Required]
      public double Y { get; set; }
      public bool RestartSolver { get; set; }
   }
}