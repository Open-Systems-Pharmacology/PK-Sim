using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Formulation : ParameterContainerSnapshotBase
   {
      [Required]
      public string FormulationType { get; set; }
   }
}