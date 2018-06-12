using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class FormulationSelection : IWithName
   {
      [Required]
      public string Name { get; set; }

      [Required]
      public string Key { get; set; }
   }
}