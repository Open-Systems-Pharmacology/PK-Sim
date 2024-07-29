using OSPSuite.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ITableSolubilityParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableSolubilityParameterPresenter : Parameters.TableParameterPresenter<ITableParameterView>, ITableSolubilityParameterPresenter
   {
      public TableSolubilityParameterPresenter(ITableParameterView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, ICompoundAlternativeTask compoundAlternativeTask) :
         base(view, parameterTask, formulaFactory, cloner, compoundAlternativeTask.ImportSolubilityTableFormula)
      {
      }
   }
}