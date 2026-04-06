using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots;

public class ParameterValue
{
   [Required]
   public string Path { get; set; }

   [Required]
   public double Value { get; set; }
}