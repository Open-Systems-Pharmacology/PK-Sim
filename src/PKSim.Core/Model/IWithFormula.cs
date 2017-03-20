namespace PKSim.Core.Model
{
   public interface IWithFormula
   {
      string CalculationMethod { get; set; }
      string Rate { get; set; }
   }
}