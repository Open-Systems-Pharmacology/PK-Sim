using PKSim.Core.Model;
using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class PkaType
   {
      [Required]
      public CompoundType Type { get; set; }

      [Required]
      public double Pka { get; set; }

      public ValueOrigin ValueOrigin { get; set; }
   }

   public class Compound : ParameterContainerSnapshotBase, IBuildingBlockSnapshot
   {
      public bool? IsSmallMolecule { get; set; }
      public PlasmaProteinBindingPartner? PlasmaProteinBindingPartner { get; set; }
      public Alternative[] Lipophilicity { get; set; }
      public Alternative[] FractionUnbound { get; set; }
      public Alternative[] Solubility { get; set; }
      public Alternative[] IntestinalPermeability { get; set; }
      public Alternative[] Permeability { get; set; }
      public PkaType[] PkaTypes { get; set; }
      public CompoundProcess[] Processes { get; set; }
      public CalculationMethodCache CalculationMethods { get; set; }
      public PKSimBuildingBlockType BuildingBlockType { get; } = PKSimBuildingBlockType.Compound;
   }
}