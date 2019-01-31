using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Formulation : ParameterContainerSnapshotBase, IBuildingBlockSnapshot
   {
      [Required]
      public string FormulationType { get; set; }

      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Formulation;
   }
}