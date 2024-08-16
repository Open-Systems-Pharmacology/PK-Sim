using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ITableSolubilityParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableSolubilityParameterPresenter : TableParameterPresenter<ITableFormulaView>, ITableSolubilityParameterPresenter
   {
      public TableSolubilityParameterPresenter(ITableFormulaView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, ICompoundAlternativeTask compoundAlternativeTask) :
         base(view, parameterTask, formulaFactory, cloner, compoundAlternativeTask.ImportSolubilityTableFormula)
      {
      }
   }
}