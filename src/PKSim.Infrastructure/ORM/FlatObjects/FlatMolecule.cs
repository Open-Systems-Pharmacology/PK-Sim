using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatMolecule : FlatObject, IWithFormula
   {
      public bool IsFloating { get; set; }
      public string Rate { get; set; }
      public string CalculationMethod { get; set; }
      public QuantityType MoleculeType { get; set; }
   }
}