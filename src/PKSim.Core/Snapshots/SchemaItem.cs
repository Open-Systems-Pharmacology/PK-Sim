using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class SchemaItem : ParameterContainerSnapshotBase
   {
      [Required]
      public string ApplicationType { get; set; }

      public string FormulationKey { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; set; }
      public string EventKey { get; set; }
   }
}