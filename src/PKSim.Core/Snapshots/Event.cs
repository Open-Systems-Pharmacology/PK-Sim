using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Event: ParameterContainerSnapshotBase
   {
      [Required]
      public string Template { get;  set; }
   }
}