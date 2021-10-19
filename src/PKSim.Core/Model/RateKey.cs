namespace PKSim.Core.Model
{
   public class RateKey : CompositeKey
   {
      public string CalculationMethod => KeyElement1;
      public string Rate => KeyElement2;

      public RateKey(string calculationMethod, string rate) : base(calculationMethod, rate)
      {
      }

      public bool IsBlackBoxFormula => CalculationMethod.Equals(CoreConstants.CalculationMethod.BlackBox);

      public bool IsTableWithOffsetFormula => Rate.StartsWith(CoreConstants.Rate.TableFormulaWithOffsetPrefix);

      public bool IsTableWithXArgumentFormula => Rate.StartsWith(CoreConstants.Rate.TableFormulaWithXArgumentPrefix);

      public bool IsDynamicSumFormula => CalculationMethod.Equals(CoreConstants.CalculationMethod.DYNAMIC_SUM_FORMULAS);
   }
}