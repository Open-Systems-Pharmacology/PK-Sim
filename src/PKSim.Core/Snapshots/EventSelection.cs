using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class EventSelection : SnapshotBase
   {
      [Required]
      public Parameter StartTime { get; set; }
   }
}