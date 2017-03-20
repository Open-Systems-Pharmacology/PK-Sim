using PKSim.Core.Model;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatEventChangedObject : FlatObjectPath,IWithFormula
   {
      public int Id { get; set; }
      public string Type { get; set; }
      public string Name { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public bool UseAsValue { get; set; }
   }
}
