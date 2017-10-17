using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class CompoundProcessSelection : IWithName
   {
      [Required]
      public string Name { get; set; }
      public string MoleculeName { get; set; }
      public string MetaboliteName { get; set; }
      public string CompoundName { get; set; }
   }
}