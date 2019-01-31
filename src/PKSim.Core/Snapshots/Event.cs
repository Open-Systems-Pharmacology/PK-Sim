using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Event: ParameterContainerSnapshotBase, IBuildingBlockSnapshot
   {
      [Required]
      public string Template { get;  set; }

      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Event;
   }
}