using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views.Parameters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Views.Parameters;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface ITableParameterPresenter : ITableFormulaPresenter
   {
      void Edit(IParameter tableParameter);
      void Save();
      TableFormula EditedFormula { get; }
   }

   public abstract class TableParameterPresenter<TView> : TableFormulaPresenter<TView>, ITableParameterPresenter where TView : ITableFormulaView
   {
      private readonly IParameterTask _parameterTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly ICloner _cloner;
      private IParameter _tableParameter;

      protected TableParameterPresenter(TView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner)
         : base(view)
      {
         _parameterTask = parameterTask;
         _formulaFactory = formulaFactory;
         _cloner = cloner;
         view.ImportVisible = false;
      }

      protected TableFormula NewTableFormula()
      {
         return _formulaFactory.CreateTableFormula();
      }

      protected TableFormula CreateClone(TableFormula tableFormula)
      {
         return _cloner.Clone(tableFormula);
      }

      protected ICommand SetParameterFormula(IParameter tableParameter, TableFormula tableFormula)
      {
         return _parameterTask.SetParameterFormula(tableParameter, tableFormula);
      }

      protected override void ApplyImportedTablePoints(DataRepository importedTablePoints)
      {
         Edit(TablePointsToTableFormula(importedTablePoints));
      }

      protected abstract TableFormula TablePointsToTableFormula(DataRepository importedTablePoints);

      public override void SetXValue(ValuePointDTO valuePointDTO, double newValue) => valuePointDTO.X = newValue;

      public override void SetYValue(ValuePointDTO valuePointDTO, double newValue) => valuePointDTO.Y = newValue;

      protected override IParameter Owner => _tableParameter;

      public void Edit(IParameter tableParameter)
      {
         _tableParameter = tableParameter;
         _view.Editable = _tableParameter.Editable;
         // setting Editable will reveal the import button so ImportVisible must be set after
         _view.ImportVisible = CanImport;
         //do not edit the parameter formula itself as the user might cancel the edit
         var tableFormula = tableParameter.Formula as TableFormula;

         if (tableFormula != null)
            tableFormula = CreateClone(tableFormula);

         tableFormula = tableFormula ?? CreateTableFormula();
         Edit(tableFormula ?? CreateTableFormula());
      }

      public abstract bool CanImport { get; }

      protected virtual TableFormula CreateTableFormula()
      {
         var formula = NewTableFormula().WithName(OwnerName);
         //use whatever default were created in the factory
         formula.InitializedWith(formula.XName, OwnerName, formula.XDimension, _tableParameter.Dimension);

         ConfigureCreatedTableAction(formula);

         return formula;
      }

      public void Save()
      {
         //do not use AddCommand here as we only want to save the table without notifying any change events
         //since all changed were performed already
         CommandCollector.AddCommand(SetParameterFormula(_tableParameter, EditedFormula));
      }

      public override bool CanClose
      {
         get
         {
            if (_editedFormula == null)
               return base.CanClose;

            return base.CanClose && _editedFormula.AllPoints.Any();
         }
      }

      public override void AddPoint()
      {
         var newPoint = new ParameterValuePointDTO(Owner, _editedFormula, new ValuePoint(double.NaN, double.NaN));
         try
         {
            _tableFormulaDTO.AllPoints.Add(newPoint);
         }
         catch (ValuePointAlreadyExistsForPointException)
         {
            _tableFormulaDTO.AllPoints.Remove(newPoint);
            throw;
         }

         _view.EditPoint(newPoint);
      }

      public TableFormula EditedFormula
      {
         get
         {
            _editedFormula.ClearPoints();
            _tableFormulaDTO.AllPoints.Each(p => _editedFormula.AddPoint(valuePointFrom(p)));
            return _editedFormula;
         }
      }

      private ValuePoint valuePointFrom(ValuePointDTO valuePointDTO)
      {
         //values are saved in display unit
         return new ValuePoint(
            _editedFormula.XDimension.UnitValueToBaseUnitValue(_editedFormula.XDisplayUnit, valuePointDTO.X),
            _editedFormula.Dimension.UnitValueToBaseUnitValue(_editedFormula.YDisplayUnit, valuePointDTO.Y));
      }

      public override void RemovePoint(ValuePointDTO pointToRemove) => _tableFormulaDTO.AllPoints.Remove(pointToRemove);
   }

   public class TableParameterPresenter : TableParameterPresenter<ITableFormulaView>, ITableParameterPresenter
   {
      public TableParameterPresenter(ITableFormulaView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner) :
         base(view, parameterTask, formulaFactory, cloner)
      {
         // default import function disabled when context is not specified
         view.ImportVisible = CanImport;
      }

      protected override DataRepository ImportTablePoints()
      {
         // default import function disabled when context is not specified
         return null;
      }

      protected override TableFormula TablePointsToTableFormula(DataRepository importedTablePoints)
      {
         // default import function disabled when context is not specified
         return null;
      }

      public override bool CanImport => false;
   }
}