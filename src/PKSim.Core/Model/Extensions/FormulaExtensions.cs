using System.Linq;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.Extensions
{
   public static class FormulaExtensions
   {
      public static bool ContainsTimePath(this IFormula formula)
      {
         return formula.ObjectPaths.Any(path => path as TimePath != null);
      }
   }
}