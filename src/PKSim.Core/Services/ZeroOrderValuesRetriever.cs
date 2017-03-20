using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class ZeroOrderValuesRetriever : FormulationValuesSpecification
   {
      private double _tEnd;

      public ZeroOrderValuesRetriever(IFormulaFactory formulaFactory) : base(formulaFactory)
      {
      }

      protected override void CacheParameterValueFor(Formulation formulation)
      {
         _tEnd = formulation.Parameter(CoreConstants.Parameter.T_END).Value;
      }

      protected override double ValueFor(Formulation formulation, double time)
      {
         return time <= _tEnd ? time / _tEnd : 1;
      }

      public override bool IsSatisfiedBy(Formulation formulation)
      {
         return formulation.FormulationType == CoreConstants.Formulation.ZeroOrder;
      }
   }
}