using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Views;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationEventsConfigurationView : BaseUserControlWithValueInGrid, ISimulationEventsConfigurationView
   {
      private ISimulationEventsConfigurationPresenter _presenter;
      private readonly GridViewBinder<EventMappingDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
      private readonly UxRepositoryItemComboBox _eventRepository;
      private readonly ComboBoxUnitParameter _comboBoxUnit;
      private IGridViewBoundColumn<EventMappingDTO, double> _columnValue;
      private IGridViewColumn _colCreateEvent;
      private IGridViewColumn _colLoadEvent;
      private IGridViewColumn _colRemoveEvent;

      public SimulationEventsConfigurationView()
      {
         InitializeComponent();
         _eventRepository = new UxRepositoryItemComboBox(gridView);
         _gridViewBinder = new GridViewBinder<EventMappingDTO>(gridView);
         _comboBoxUnit = new ComboBoxUnitParameter(gridControl);
         gridView.AllowsFiltering = false;
         InitializeWithGrid(gridView);
      }

      public override void InitializeBinding()
      {
         var createButtonRepository = createEventButtonRepository();
         var loadButtonRepository = loadEventButtonRepository();

         _columnValue = _gridViewBinder.Bind(param => param.StartTime);
         _columnValue.WithFormat(eventMappingDTO => eventMappingDTO.StartTimeParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.StartTime)
            .WithEditorConfiguration((activeEditor, eventMappingDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, eventMappingDTO.StartTimeParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.StartTimeParameter, valueInGuiUnit.NewValue);

         _gridViewBinder.AutoBind(x => x.Event)
            .WithRepository(x => _eventRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithEditorConfiguration(configureEventRepository);

        _colCreateEvent= _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => createButtonRepository);

        _colLoadEvent = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => loadButtonRepository);

        _colRemoveEvent =  _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => _removeButtonRepository);

         btnAddEvent.Click += (o, e) => OnEvent(_presenter.AddEventMapping);
         _comboBoxUnit.ParameterUnitSet += setParameterUnit;
         gridView.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
         _removeButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.RemoveEventMapping(_gridViewBinder.FocusedElement));

         loadButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.LoadEventAsync(_gridViewBinder.FocusedElement));
         createButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.CreateEventFor(_gridViewBinder.FocusedElement));
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         if (column == null) return false;
         return _columnValue.XtraColumn == column;
      }

      protected override bool ColumnIsButton(GridColumn column)
      {
         return column.IsOneOf(_colCreateEvent.XtraColumn, _colLoadEvent.XtraColumn, _colRemoveEvent.XtraColumn);
      }

      public void RefreshData()
      {
         gridView.RefreshData();
      }

      private RepositoryItemButtonEdit createEventButtonRepository()
      {
         return new UxRepositoryItemButtonImage(ApplicationIcons.Create,PKSimConstants.UI.CreateBuildingBlockHint(PKSimConstants.ObjectTypes.Event));
      }

      private RepositoryItemButtonEdit loadEventButtonRepository()
      {
         return new UxRepositoryItemButtonImage(ApplicationIcons.LoadFromTemplate, PKSimConstants.UI.LoadItemFromTemplate(PKSimConstants.ObjectTypes.Event));
      }

      private void setParameterValue(IParameterDTO parameterDTO, double newValue)
      {
         OnEvent(() => _presenter.SetParameterValue(parameterDTO, newValue));
      }

      private void setParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         OnEvent(() => _presenter.SetParameterUnit(parameterDTO, newUnit));
      }

      private void configureEventRepository(BaseEdit baseEdit, EventMappingDTO eventMappingDTO)
      {
         baseEdit.FillComboBoxEditorWith(_presenter.AllEvents());
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnAddEvent.InitWithImage(ApplicationIcons.Add, PKSimConstants.UI.AddEvent);
         layoutItemAddEvent.AdjustButtonSize();
      }

      public void AttachPresenter(ISimulationEventsConfigurationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Event;

      public override string Caption => PKSimConstants.UI.SimulationEventsConfiguration;

      public void BindTo(IEnumerable<EventMappingDTO> allEventsMappingDTO)
      {
         _gridViewBinder.BindToSource(allEventsMappingDTO);
      }

      public override bool HasError => _gridViewBinder.HasError;
   }
}