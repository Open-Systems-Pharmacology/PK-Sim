using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class ProtocolSelection : IWithName
   {
      [Required]
      public string Name { get; set; }

      public FormulationSelection[] Formulations { get; set; }
   }
}