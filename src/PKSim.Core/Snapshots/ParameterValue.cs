using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots;

public class ParameterValue
{
   [Required]
   public string Path { get; set; }

   [Required]
   public double Value { get; set; }

   public string Dimension { get; set; }

   public string Unit { get; set; }

   public double? MinValue { get; set; }

   public bool? MinIsAllowed { get; set; }

   public double? MaxValue { get; set; }

   public bool? MaxIsAllowed { get; set; }
}
