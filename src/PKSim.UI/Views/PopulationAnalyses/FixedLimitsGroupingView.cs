using System.Collections.Generic;
using System.ComponentModel;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.RepositoryItems;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.UI;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class FixedLimitsGroupingView : BaseUserControl, IFixedLimitsGroupingView
   {
      private IFixedLimitsGroupingPresenter _presenter;
      private readonly GridViewBinder<FixedLimitGroupingDTO> _gridViewBinder;
      protected readonly RepositoryItemButtonEdit _addAndRemoveButtonRepository = new UxAddAndRemoveButtonRepository();
      protected readonly RepositoryItemButtonEdit _addButtonRepository = new UxAddAndDisabledRemoveButtonRepository();
      protected readonly RepositoryItemButtonEdit _disableRemoveButtonRepository = new UxDisableAddAndDisableRemoveButtonRepository();
      private IGridViewColumn _colMinimum;
      private IGridViewColumn _colMaximum;
      private readonly UxRepositoryItemComboBox _symbolsRepository;
      private readonly UxRepositoryItemColorPickEditWithHistory _colorRepository;

      public FixedLimitsGroupingView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<FixedLimitGroupingDTO>(gridView);
         gridView.RowCellStyle += (o, e) => OnEvent(updateRowCellStyle,e);
         gridView.ShowingEditor += (o, e) => OnEvent(onShowingEditor,e);
         gridView.ShowRowIndicator = false;
         gridView.AllowsFiltering = false;
         _symbolsRepository = new UxSymbolsComboBoxRepository(gridView) ;
         _colorRepository = new UxRepositoryItemColorPickEditWithHistory();
      }

      public void AttachPresenter(IFixedLimitsGroupingPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<FixedLimitGroupingDTO> fixedLimitDTOs, Unit fieldUnit)
      {
         _gridViewBinder.BindToSource(fixedLimitDTOs);
         _colMinimum.Caption = Constants.NameWithUnitFor(PKSimConstants.UI.Minimum, fieldUnit);
         _colMaximum.Caption = Constants.NameWithUnitFor(PKSimConstants.UI.Maximum, fieldUnit);
         gridView.BestFitColumns();
      }

      public override void InitializeBinding()
      {
         //use autobind to enable automatic notification
         _colMinimum = _gridViewBinder.AutoBind(x => x.Minimum);
         _colMaximum = _gridViewBinder.AutoBind(x => x.Maximum)
            .WithOnValueSet((o, e) => OnEvent(() => maximumValueChanged(o,e.NewValue)));

         _gridViewBinder.AutoBind(x => x.Label);
         _gridViewBinder.AutoBind(x => x.Color)
            .WithRepository(x => _colorRepository);
         _gridViewBinder.AutoBind(x => x.Symbol)
            .WithRepository(x => _symbolsRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(getButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _addButtonRepository.ButtonClick += (o, e) => OnEvent(addFixedLimit);
         _addAndRemoveButtonRepository.ButtonClick += (o, e) => OnEvent(addOrRemoveFixedLimit,e);

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      private void maximumValueChanged(FixedLimitGroupingDTO dto, double? newValue)
      {
         _presenter.MaximumValueChanged(dto, newValue);
         //Required so that changed are being propagated to data source (only works when leaving row otherwise)
         gridControl.RefreshDataSource();
      }

      private void addOrRemoveFixedLimit(ButtonPressedEventArgs e)
      {
         if (e.Button.Kind == ButtonPredefines.Delete)
            _presenter.RemoveFixedLimit(_gridViewBinder.FocusedElement);
         else
            addFixedLimit();
      }

      private void addFixedLimit()
      {
         _presenter.AddFixedLimitAfter(_gridViewBinder.FocusedElement);
      }

      private void updateRowCellStyle(RowCellStyleEventArgs e)
      {
         var dto = _gridViewBinder.ElementAt(e.RowHandle);
         if (dto == null) return;

         //minimum can never be edited
         if (e.Column == _colMinimum.XtraColumn)
            gridView.AdjustAppearance(e, false);

         else if (e.Column == _colMaximum.XtraColumn)
            gridView.AdjustAppearance(e, dto.MaximumEditable);
      }

      private void onShowingEditor(CancelEventArgs e)
      {
         //unit cannot be edited for discrete parameters;
         var dto = _gridViewBinder.FocusedElement;
         if (dto == null) return;

         //minimum can never be edited
         if (gridView.FocusedColumn == _colMinimum.XtraColumn)
            e.Cancel = true;

         else if (gridView.FocusedColumn == _colMaximum.XtraColumn)
            e.Cancel = !dto.MaximumEditable;
      }

      private RepositoryItem getButtonRepository(FixedLimitGroupingDTO groupingDTO)
      {
         if (groupingDTO.CanDelete)
            return _addAndRemoveButtonRepository;

         if (groupingDTO.CanAdd)
            return _addButtonRepository;

         return _disableRemoveButtonRepository;
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError; }
      }
   }
}