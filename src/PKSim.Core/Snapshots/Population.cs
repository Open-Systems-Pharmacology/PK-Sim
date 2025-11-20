using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class Population : SnapshotBase, IBuildingBlockSnapshot
   {
      public int? Seed { get; set; }

      [Required]
      public PopulationSettings Settings { get; set; }
      public AdvancedParameter[] AdvancedParameters { get; set; }
      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Population;
   }
}