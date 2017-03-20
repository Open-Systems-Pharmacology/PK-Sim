using PKSim.Core.Model;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatEventCondition:FlatContainerId ,IWithFormula
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public bool IsOneTime { get; set; }
   }
}
