using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots;

public interface IBuildingBlockSnapshot : IWithName
{
   PKSimBuildingBlockType BuildingBlockType { get; }
}