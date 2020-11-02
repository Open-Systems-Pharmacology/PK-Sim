using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Core.Snapshots
{
   public class Molecule : ParameterContainerSnapshotBase
   {
      [Required]
      public QuantityType Type { get; set; }

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
   }
}