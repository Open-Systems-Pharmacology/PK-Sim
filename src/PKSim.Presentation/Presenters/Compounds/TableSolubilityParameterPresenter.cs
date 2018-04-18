using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ITableSolubilityParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableSolubilityParameterPresenter : TableParameterPresenter<ITableParameterView>, ITableSolubilityParameterPresenter
   {
      public TableSolubilityParameterPresenter(ITableParameterView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, ICompoundAlternativeTask compoundAlternativeTask) :
         base(view, parameterTask, formulaFactory, cloner, compoundAlternativeTask.ImportSolubilityTableFormula)
      {
      }
   }
}	