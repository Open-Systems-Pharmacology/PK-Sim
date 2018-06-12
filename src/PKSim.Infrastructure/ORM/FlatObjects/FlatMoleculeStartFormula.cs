namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatMoleculeStartFormula : FlatContainerId
   {
      public string MoleculeName { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public int? ValueOriginId { get; set; }
   }
}
