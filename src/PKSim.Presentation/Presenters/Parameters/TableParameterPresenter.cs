using System;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Presenters.Parameters
{
   public abstract class TableParameterPresenter<TView> : OSPSuite.Presentation.Presenters.Parameters.TableParameterPresenter<TView> where TView : ITableParameterView
   {
      private readonly IParameterTask _parameterTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly ICloner _cloner;

      protected TableParameterPresenter(TView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, Func<TableFormula> importTableFormula)
         : base(view, importTableFormula)
      {
         _parameterTask = parameterTask;
         _formulaFactory = formulaFactory;
         _cloner = cloner;
      }

      protected override TableFormula NewTableFormula()
      {
         return _formulaFactory.CreateTableFormula();
      }

      protected override TableFormula CreateClone(TableFormula tableFormula)
      {
         return _cloner.Clone(tableFormula);
      }

      protected override ICommand SetParameterFormula(IParameter tableParameter, TableFormula tableFormula)
      {
         return _parameterTask.SetParameterFormula(tableParameter, tableFormula);
      }
   }

   public class TableParameterPresenter : TableParameterPresenter<ITableParameterView>
   {
      public TableParameterPresenter(ITableParameterView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner) :
         base(view, parameterTask, formulaFactory, cloner, () => formulaFactory.CreateTableFormula())
      {
         //default import function disabled when context is not specified
         view.ImportVisible = false;
      }
   }
}