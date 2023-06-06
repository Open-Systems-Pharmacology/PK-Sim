using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Core.Snapshots
{
   public class ExpressionProfile : SnapshotBase, IBuildingBlockSnapshot
   {
      [Required] public QuantityType Type { get; set; }

      [Required] public string Species { get; set; }

      [Required] public string Molecule { get; set; }

      [Required] public string Category { get; set; }

      public LocalizedParameter[] Parameters { get; set; }

      //Proteins only for compatibility with old snapshots
      public MembraneLocation? MembraneLocation { get; set; }
      public TissueLocation? TissueLocation { get; set; }
      public IntracellularVascularEndoLocation? IntracellularVascularEndoLocation { get; set; }

      //Proteins only 
      public Localization? Localization { get; set; }

      //Transporters only
      public TransportType? TransportType { get; set; }

      public ExpressionContainer[] Expression { get; set; }
      public Ontogeny Ontogeny { get; set; }

      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.ExpressionProfile;

      //Null if no disease state defined
      public DiseaseState Disease { get; set; }
   }
}