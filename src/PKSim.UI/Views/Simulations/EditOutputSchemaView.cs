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

      public override string Caption
      {
         get { return PKSimConstants.UI.SimulationSettings; }
      }

      public override void InitializeBinding()
      {
         _gridViewBinder = new GridViewBinder<OutputIntervalDTO>(mainView) {BindingMode = BindingMode.OneWay, ValidationMode = ValidationMode.LeavingRow};

         _gridViewBinder.Bind(x => x.StartTime)
            .WithCaption(PKSimConstants.UI.StartTime)
            .WithFormat(x => x.StartTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.StartTimeParameter))
            .OnValueSet += (dto, valueInGuiUnit) => setParameterValue(dto.StartTimeParameter, valueInGuiUnit.NewValue);

         _gridViewBinder.Bind(x => x.EndTime)
            .WithCaption(PKSimConstants.UI.EndTime)
            .WithFormat(x => x.EndTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.EndTimeParameter))
            .OnValueSet += (dto, valueInGuiUnit) => setParameterValue(dto.EndTimeParameter, valueInGuiUnit.NewValue);

         _gridViewBinder.Bind(x => x.Resolution)
            .WithFormat(x => x.ResolutionParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.NumberOfTimePoints)
            .WithEditorConfiguration((activeEditor, intervalDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, intervalDTO.ResolutionParameter))
            .OnValueSet += (dto, valueInGuiUnit) => setParameterValue(dto.ResolutionParameter, valueInGuiUnit.NewValue);

         _buttonColumn = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(getButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _addAndRemoveButtonRepository.ButtonClick += (o, e) => OnEvent(() => addRemoveButtonClick(o, e));
         _addButtonRepository.ButtonClick += (o, e) => OnEvent(addButtonClick);


         _comboBoxUnit.ParameterUnitSet += setParameterUnit;
         mainView.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         if (column == null) return false;
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

      private void addRemoveButtonClick(object sender, ButtonPressedEventArgs e)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
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

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.OutputInterval; }
      }

      public void BindTo(IEnumerable<OutputIntervalDTO> allIntervals)
      {
         _gridViewBinder.BindToSource(allIntervals);
         mainView.BestFitColumns();
      }
   }
}