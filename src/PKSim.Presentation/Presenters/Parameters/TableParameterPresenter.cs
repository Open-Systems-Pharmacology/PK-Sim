using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Views.Parameters;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface ITableParameterPresenter : ICommandCollectorPresenter
   {
      void Edit(IParameter tableParameter);
      void ImportTable();
      void RemovePoint(ValuePointDTO pointToRemove);
      void Save();
      void AddPoint();
      TableFormula EditedFormula { get; }
      IEnumerable<ValuePointDTO> AllPoints();
      void SetXValue(ValuePointDTO valuePointDTO, double newValue);
      void SetYValue(ValuePointDTO valuePointDTO, double newValue);

      /// <summary>
      ///    Action that can be called to configure the created <see cref="TableFormula" />
      /// </summary>
      Action<TableFormula> ConfigureCreatedTableAction { get; set; }

      string Description { get; set; }

      string ImportToolTip { get; set; }
   }

   public abstract class TableParameterPresenter<TView> : AbstractCommandCollectorPresenter<TView, ITableParameterPresenter>, ITableParameterPresenter
      where TView : ITableParameterView
   {
      private IParameter _tableParameter;
      private readonly IParameterTask _parameterTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly ICloner _cloner;
      private readonly Func<TableFormula> _importTableFormula;
      private TableFormula _editedFormula;
      private INotifyList<ValuePointDTO> _allPoints = new NotifyList<ValuePointDTO>();
      public Action<TableFormula> ConfigureCreatedTableAction { get; set; }

      protected TableParameterPresenter(TView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, Func<TableFormula> importTableFormula)
         : base(view)
      {
         _parameterTask = parameterTask;
         _formulaFactory = formulaFactory;
         _cloner = cloner;
         _importTableFormula = importTableFormula;
         ConfigureCreatedTableAction = t => { };
      }

      public void SetXValue(ValuePointDTO valuePointDTO, double newValue)
      {
         //needed maybe if we have commands...
         valuePointDTO.X = newValue;
      }

      public void SetYValue(ValuePointDTO valuePointDTO, double newValue)
      {
         //needed maybe if we have commands...
         valuePointDTO.Y = newValue;
      }

      public string Description
      {
         set => View.Description = value;
         get => View.Description;
      }

      public string ImportToolTip
      {
         set => View.ImportToolTip = value;
         get => View.ImportToolTip;
      }

      public TableFormula EditedFormula
      {
         get
         {
            _editedFormula.ClearPoints();
            _allPoints.Each(p => _editedFormula.AddPoint(valuePointFrom(p)));
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

      public void Edit(IParameter tableParameter)
      {
         _tableParameter = tableParameter;
         _view.Editable = _tableParameter.Editable;
         //do not edit the parameter formula itself as the user might cancel the edit
         var tableFormula = tableParameter.Formula as TableFormula;
         if (tableFormula != null)
            tableFormula = _cloner.Clone(tableFormula);

         editFormula(tableFormula);
      }

      private void editFormula(TableFormula tableFormula)
      {
         _editedFormula = tableFormula ?? CreateTableFormula();
         if (_allPoints != null)
            _allPoints.CollectionChanged -= notifyChange;

         _allPoints = new NotifyList<ValuePointDTO>();
         _editedFormula.AllPoints().Each(p => _allPoints.Add(new ValuePointDTO(_tableParameter, _editedFormula, p)));

         var yName = string.IsNullOrEmpty(_editedFormula.YName) ? _tableParameter.Name : _editedFormula.YName;
         _view.XCaption = Constants.NameWithUnitFor(_editedFormula.XName, _editedFormula.XDisplayUnit);
         _view.YCaption = Constants.NameWithUnitFor(yName, _editedFormula.YDisplayUnit);

         _view.BindTo(_allPoints);

         _allPoints.CollectionChanged += notifyChange;
         if (tableFormula == null)
            ViewChanged();
      }

      protected virtual TableFormula CreateTableFormula()
      {
         var formula = _formulaFactory.CreateTableFormula().WithName(_tableParameter.Name);
         //use whatever default were created in the factory
         formula.InitializedWith(formula.XName, _tableParameter.Name, formula.XDimension, _tableParameter.Dimension);

         ConfigureCreatedTableAction(formula);

         return formula;
      }

      private void notifyChange(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
      {
         ViewChanged();
      }

      public override bool CanClose
      {
         get
         {
            if (_editedFormula == null)
               return base.CanClose;

            return base.CanClose && _editedFormula.AllPoints().Any();
         }
      }

      public void ImportTable()
      {
         var importedFormula = _importTableFormula();
         if (importedFormula == null)
            return;

         editFormula(importedFormula);
         ViewChanged();
      }

      public void RemovePoint(ValuePointDTO pointToRemove)
      {
         _allPoints.Remove(pointToRemove);
      }

      public void Save()
      {
         //do not use AddCommand here as we only want to save the table without notifying any change events
         //since all changed were performed already
         CommandCollector.AddCommand(_parameterTask.SetParameterFormula(_tableParameter, EditedFormula));
      }

      public void AddPoint()
      {
         var newPoint = new ValuePointDTO(_tableParameter, _editedFormula, new ValuePoint(double.NaN, double.NaN));
         try
         {
            _allPoints.Add(newPoint);
         }
         catch (ValuePointAlreadyExistsForPointException)
         {
            _allPoints.Remove(newPoint);
            throw;
         }

         _view.EditPoint(newPoint);
      }

      public IEnumerable<ValuePointDTO> AllPoints()
      {
         return _allPoints;
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