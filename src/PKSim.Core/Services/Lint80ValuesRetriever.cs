using System;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class Lint80ValuesRetriever : FormulationValuesSpecification
   {
      private double _dissTime80;
      private double _lagTime;

      public Lint80ValuesRetriever(IFormulaFactory formulaFactory) : base(formulaFactory)
      {
      }

      protected override void CacheParameterValueFor(Formulation formulation)
      {
         _lagTime = formulation.Parameter(CoreConstants.Parameters.LAG_TIME).Value;
         _dissTime80 = formulation.Parameter(CoreConstants.Parameters.DISS_TIME80).Value;
      }

      protected override double ValueFor(Formulation formulation, double time)
      {
         if (time <= _lagTime)
            return 0;

         if (time - _lagTime <= _dissTime80)
            return 0.8 * (time - _lagTime) / _dissTime80;

         if (time - _lagTime < 3.0 / 2 * _dissTime80)
            return 1 - 0.8 * Math.Pow((time - _lagTime - 3.0 / 2 * _dissTime80) / _dissTime80, 2);

         return 1;
      }

      public override bool IsSatisfiedBy(Formulation formulation)
      {
         return formulation.FormulationType == CoreConstants.Formulation.Lint80;
      }
   }
}