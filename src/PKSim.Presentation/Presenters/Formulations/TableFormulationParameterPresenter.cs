using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ITableFormulationParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableFormulationParameterPresenter : TableParameterPresenter<ITableFormulaView>, ITableFormulationParameterPresenter
   {
      private readonly IFormulationTask _formulationTask;

      public TableFormulationParameterPresenter(ITableFormulaView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, IFormulationTask formulationTask) :
         base(view, parameterTask, formulaFactory, cloner)
      {
         _formulationTask = formulationTask;
      }

      protected override TableFormula TablePointsToTableFormula(DataRepository importedTablePoints)
      {
         return _formulationTask.TableFormulaFrom(importedTablePoints);
      }

      protected override DataRepository ImportTablePoints()
      {
         return _formulationTask.ImportTablePoints();
      }
   }
}