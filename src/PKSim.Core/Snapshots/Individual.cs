using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Individual : SnapshotBase
   {
      public int? Seed { get; set; }

      [Required]
      public string Species { get; set; }

      [Required]
      public string Population { get; set; }

      [Required]
      public string Gender { get; set; }

      public Parameter Age { get; set; }
      public Parameter GestationalAge { get; set; }
      public Parameter Weight { get; set; }
      public Parameter Height { get; set; }

      public LocalizedParameter[] Parameters { get; set; }
      public Molecule[] Molecules { get; set; }
   }
}