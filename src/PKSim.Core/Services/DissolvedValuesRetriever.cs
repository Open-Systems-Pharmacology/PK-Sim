using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class DissolvedValuesRetriever : FormulationValuesSpecification
   {
      public DissolvedValuesRetriever(IFormulaFactory formulaFactory) : base(formulaFactory)
      {
      }

      protected override void CacheParameterValueFor(Formulation formulation)
      {
         /*nothng to do */
      }

      protected override double ValueFor(Formulation formulation, double time)
      {
         return 1;
      }

      public override bool IsSatisfiedBy(Formulation formulation)
      {
         return formulation.FormulationType == CoreConstants.Formulation.DISSOLVED;
      }
   }
}