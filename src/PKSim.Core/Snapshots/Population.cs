using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Population : SnapshotBase
   {
      public int? Seed { get; set; }

      [Required]
      public PopulationSettings Settings { get; set; }
      public AdvancedParameter[] AdvancedParameters { get; set; }
   }
}