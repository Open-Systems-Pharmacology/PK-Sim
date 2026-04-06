using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class OverwriteParameterSetSelection
   {
      [Required]
      public string CompoundName { get; set; }

      [Required]
      public string OverwriteParameterSetName { get; set; }
   }
}
