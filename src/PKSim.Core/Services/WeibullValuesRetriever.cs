using System;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class WeibullValuesRetriever : FormulationValuesSpecification
   {
      private double _dissTime50;
      private double _lagTime;
      private double _dissShape;
      private double _tau;

      public WeibullValuesRetriever(IFormulaFactory formulaFactory) : base(formulaFactory)
      {
      }

      protected override void CacheParameterValueFor(Formulation formulation)
      {
         _dissTime50 = formulation.Parameter(CoreConstants.Parameters.DISS_TIME50).Value;
         _lagTime = formulation.Parameter(CoreConstants.Parameters.LAG_TIME).Value;
         _dissShape = formulation.Parameter(CoreConstants.Parameters.DISS_SHAPE).Value;

         _tau = Math.Pow(_dissTime50, _dissShape) / Math.Log(2);
      }

      protected override double ValueFor(Formulation formulation, double time)
      {
         if (time - _lagTime <= 0)
            return 0;

         return 1 - Math.Exp((-Math.Pow(time - _lagTime, _dissShape)) / _tau);
      }

      public override bool IsSatisfiedBy(Formulation formulation)
      {
         return formulation.FormulationType == CoreConstants.Formulation.Weibull;
      }
   }
}