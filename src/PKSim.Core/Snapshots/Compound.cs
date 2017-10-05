using PKSim.Core.Model;

namespace PKSim.Core.Snapshots
{
   public class PkaType
   {
      public CompoundType Type { get; set; }
      public double Pka { get; set; }
   }

   public class Compound : ParameterContainerSnapshotBase
   {
      public bool? IsSmallMolecule { get; set; }
      public PlasmaProteinBindingPartner?PlasmaProteinBindingPartner { get; set; }
      public Alternative[] Lipophilicity { get; set; }
      public Alternative[] FractionUnbound { get; set; }
      public Alternative[] Solubility { get; set; }
      public Alternative[] IntestinalPermeability { get; set; }
      public Alternative[] Permeability { get; set; }
      public PkaType[] PkaTypes { get; set; }
      public CompoundProcess[] Processes { get; set; }
      public CalculationMethodCache CalculationMethods { get; set; }
   }
}