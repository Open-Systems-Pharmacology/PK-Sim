using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class Event: ParameterContainerSnapshotBase, IBuildingBlockSnapshot
   {
      [Required]
      public string Template { get;  set; }

      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Event;
   }
}