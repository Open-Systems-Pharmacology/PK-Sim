using System.Collections.Generic;

namespace PKSim.Core.Batch
{
   internal class PkaType
   {
      public string Type { get; set; }
      public double Value { get; set; }
   }

   internal class Compound
   {
      public string Name { get; set; }
      public bool IsSmallMolecule { get; set; }
      public double Lipophilicity { get; set; }
      public double FractionUnbound { get; set; }
      public double MolWeight { get; set; }
      public double Cl { get; set; }
      public double Br { get; set; }
      public double I { get; set; }
      public double F { get; set; }
      public double SolubilityAtRefpH { get; set; }
      public double RefpH { get; set; }
      public List<PkaType> PkaTypes { get; set; }
      public List<SystemicProcess> SystemicProcesses { get; set; }
      public List<PartialProcess> PartialProcesses { get; set; }
      public List<string> CalculationMethods { get; set; }

      public Compound()
      {
         CalculationMethods = new List<string>();
         PkaTypes = new List<PkaType>();
         SystemicProcesses = new List<SystemicProcess>();
         PartialProcesses = new List<PartialProcess>();
         IsSmallMolecule = true;
      }
   }
}