using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Individual : SnapshotBase, IBuildingBlockSnapshot
   {
      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Individual;
      public int? Seed { get; set; }
      public OriginData OriginData { get; set; }
      public LocalizedParameter[] Parameters { get; set; }

      //V10 and below. Expression profiles were directly defined as molecule
      public ExpressionProfile[] Molecules { get; set; }

      //v11. Expression profiles are just a reference to existing expression profiles
      public string[] ExpressionProfiles { get; set; }
   }
}