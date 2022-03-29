using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class EditOutputSchemaView : BaseUserControlWithValueInGrid, IEditOutputSchemaView
   {
      private IEditOutputSchemaPresenter _presenter;
      private GridViewBinder<OutputIntervalDTO> _gridViewBinder;
      private readonly ComboBoxUnitParameter _comboBoxUnit;
      private readonly RepositoryItemButtonEdit _addAndRemoveButtonRepository = new UxAddAndRemoveButtonRepository();
      private readonly RepositoryItemButtonEdit _addButtonRepository = new UxAddAndDisabledRemoveButtonRepository();
      private IGridViewColumn<OutputIntervalDTO> _buttonColumn;

      public EditOutputSchemaView()
      {
         InitializeComponent();
         _comboBoxUnit = new ComboBoxUnitParameter(gridOutputInterval);
         InitializeWithGrid(mainView);
      }

      public override string Caption => PKSimConstants.UI.SimulationSettings;

      public override void InitializeBinding()
      {
         _gridViewBinder = new GridViewBinder<OutputIntervalDTO>(mainView) {BindingMode = BindingMode.OneWay, ValidationMode = ValidationMode.LeavingRow};

         _gridViewBinder.Bind(x => x.StartTime)
            .WithCaption(PKSimConstants.UI.StartTime)
            .WithFormat(x => x.StartTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.StartTimeParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.StartTimeParameter, valueInGuiUnit.NewValue);

         _gridViewBinder.Bind(x => x.EndTime)
            .WithCaption(PKSimConstants.UI.EndTime)
            .WithFormat(x => x.EndTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.EndTimeParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.EndTimeParameter, valueInGuiUnit.NewValue);

         _gridViewBinder.Bind(x => x.Resolution)
            .WithFormat(x => x.ResolutionParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.NumberOfTimePoints)
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.ResolutionParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.ResolutionParameter, valueInGuiUnit.NewValue);

         _buttonColumn = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(getButtonRepository)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _addAndRemoveButtonRepository.ButtonClick += (o, e) => OnEvent(() => addRemoveButtonClick(e));
         _addButtonRepository.ButtonClick += (o, e) => OnEvent(addButtonClick);


         _comboBoxUnit.ParameterUnitSet += setParameterUnit;
         mainView.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
      }

      protected override bool ColumnIsButton(GridColumn column)
      {
         return _buttonColumn.XtraColumn == column;
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _buttonColumn.XtraColumn != column;
      }

      private RepositoryItem getButtonRepository(OutputIntervalDTO alternativeDTO)
      {
         return _presenter.CanRemoveInterval() ? _addAndRemoveButtonRepository : _addButtonRepository;
      }

      private void setParameterValue(IParameterDTO parameterDTO, double newValue)
      {
         OnEvent(() => _presenter.SetParameterValue(parameterDTO, newValue));
      }

      private void setParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         OnEvent(() =>
         {
            mainView.CloseEditor();
            _presenter.SetParameterUnit(parameterDTO, newUnit);
         });
      }

      private void addRemoveButtonClick(ButtonPressedEventArgs e)
      {
         if(e.Button==null)
            return;
         
         if (Equals(e.Button.Tag, ButtonType.Add))
            _presenter.AddOutputInterval();
         else
            _presenter.RemoveOutputInterval(_gridViewBinder.FocusedElement);

         mainView.CloseEditor();
      }

      private void addButtonClick()
      {
         _presenter.AddOutputInterval();
         mainView.CloseEditor();
      }

      public void AttachPresenter(IEditOutputSchemaPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.OutputInterval;

      public void BindTo(IEnumerable<OutputIntervalDTO> allIntervals)
      {
         _gridViewBinder.BindToSource(allIntervals);
         mainView.BestFitColumns();
      }
   }
}