namespace PKSim.Core.Model
{
   public class RateKey : CompositeKey
   {
      public string CalculationMethod
      {
         get { return KeyElement1; }
      }

      public string Rate
      {
         get { return KeyElement2; }
      }

      public RateKey(string calculationMethod, string rate) : base(calculationMethod, rate)
      {
      }

      public bool IsBlackBoxFormula
      {
         get { return CalculationMethod.Equals(CoreConstants.CalculationMethod.BlackBox); }
      }

      public bool IsTableWithOffsetFormula
      {
         get { return Rate.StartsWith(CoreConstants.Rate.TableFormulaWithOffsetPrefix); }
      }

      public bool IsDynamicSumFormula
      {
         get { return CalculationMethod.Equals(CoreConstants.CalculationMethod.DynamicSumFormulas); }
      }
   }
}