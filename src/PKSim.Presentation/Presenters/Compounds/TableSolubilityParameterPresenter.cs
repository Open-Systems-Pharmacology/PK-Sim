using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ITableSolubilityParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableSolubilityParameterPresenter : TableParameterPresenter<ITableFormulaView>, ITableSolubilityParameterPresenter
   {
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;

      public TableSolubilityParameterPresenter(ITableFormulaView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, ICompoundAlternativeTask compoundAlternativeTask) :
         base(view, parameterTask, formulaFactory, cloner)
      {
         _compoundAlternativeTask = compoundAlternativeTask;
      }

      protected override TableFormula TablePointsToTableFormula(DataRepository importedTablePoints)
      {
         return _compoundAlternativeTask.TableFormulaFrom(importedTablePoints);
      }

      protected override DataRepository ImportTablePoints()
      {
         return _compoundAlternativeTask.ImportSolubilityTablePoints();
      }
   }
}