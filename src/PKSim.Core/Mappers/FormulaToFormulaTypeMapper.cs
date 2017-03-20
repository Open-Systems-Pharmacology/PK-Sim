using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IFormulaToFormulaTypeMapper : IMapper<IFormula, FormulaType>
   {
   }

   public class FormulaToFormulaTypeMapper : IFormulaToFormulaTypeMapper
   {
      public FormulaType MapFrom(IFormula formula)
      {
         if (formula == null)
            return FormulaType.Constant;

         if (formula.IsAnImplementationOf<IDistributionFormula>())
            return FormulaType.Distribution;

         if (formula.IsAnImplementationOf<TableFormula>())
            return FormulaType.Table;

         return formula.IsConstant() ? FormulaType.Constant : FormulaType.Rate;
      }
   }
}