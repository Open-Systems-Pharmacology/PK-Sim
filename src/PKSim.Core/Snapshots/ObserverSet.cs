using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class ObserverSet : SnapshotBase, IBuildingBlockSnapshot
   {
      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.ObserverSet;
      public Observer[] Observers { get; set; }
   }
}