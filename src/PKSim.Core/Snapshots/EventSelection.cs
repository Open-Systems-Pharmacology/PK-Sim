using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class EventSelection : SnapshotBase
   {
      [Required]
      public Parameter StartTime { get; set; }
   }
}