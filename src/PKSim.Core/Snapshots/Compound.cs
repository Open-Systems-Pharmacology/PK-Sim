using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class PkaType
   {
      public string Type { get; set; }
      public double Pka { get; set; }
   }

   public class Compound : ParameterContainerSnapshotBase
   {
      public bool IsSmallMolecule { get; set; }
      public string PlasmaProteinBindingPartner { get; set; }
      public List<Alternative> Lipophilicity { get; set; } = new List<Alternative>();
      public List<Alternative> FractionUnbound { get; set; } = new List<Alternative>();
      public List<Alternative> Solubility { get; set; } = new List<Alternative>();
      public List<Alternative> IntestinalPermeability { get; set; } = new List<Alternative>();
      public List<Alternative> Permeability { get; set; } = new List<Alternative>();
      public List<PkaType> PkaTypes { get; set; } = new List<PkaType>();
      public List<CompoundProcess> Processes { get; set; } = new List<CompoundProcess>();
      public List<CalculationMethod> CalculationMethods = new List<CalculationMethod>();
   }
}