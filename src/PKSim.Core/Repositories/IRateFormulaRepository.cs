using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public class RateFormula
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public string Formula { get; set; }
      public string Dimension { get; set; }
   }

   public interface IRateFormulaRepository : IStartableRepository<RateFormula>
   {
      string FormulaFor(RateKey rateKey);
      string DimensionNameFor(RateKey rateKey);
   }
}