using System;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class FirstOrderValuesRetriever : FormulationValuesSpecification
   {
      private double _tHalf;

      public FirstOrderValuesRetriever(IFormulaFactory formulaFactory) : base(formulaFactory)
      {
      }

      public override bool IsSatisfiedBy(Formulation formulation)
      {
         return formulation.FormulationType == CoreConstants.Formulation.FIRST_ORDER;
      }

      protected override void CacheParameterValueFor(Formulation formulation)
      {
         _tHalf = formulation.Parameter(CoreConstants.Parameters.HALF_LIFE).Value;
      }

      protected override double ValueFor(Formulation formulation, double time)
      {
         return 1 - Math.Exp(-Math.Log(2) * time / _tHalf);
      }
   }
}