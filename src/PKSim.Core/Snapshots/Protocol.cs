using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots
{
   public class Protocol : ParameterContainerSnapshotBase, IBuildingBlockSnapshot
   {
      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Protocol;

      //Simple protocol properties
      public string ApplicationType { get; set; }

      public DosingIntervalId DosingInterval { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; set; }

      public bool IsSimple => !string.IsNullOrEmpty(ApplicationType);

      //Advanced protocol properties
      public Schema[] Schemas { get; set; }

      public string TimeUnit { get; set; }
   }
}