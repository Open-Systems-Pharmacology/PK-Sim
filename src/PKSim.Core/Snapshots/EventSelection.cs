using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class EventSelection : IWithName
   {
      [Required]
      public string Name { get; set; }

      [Required]
      public Parameter StartTime { get; set; }
   }
}