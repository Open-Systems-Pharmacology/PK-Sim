using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   public partial class MolWeightGroupView : BaseUserControlWithValueInGrid, IMolWeightGroupView
   {
      private readonly GridViewBinder<IParameterDTO> _gridViewBinder;
      private readonly RepositoryItemPopupContainerEdit _repositoryItemPopupContainerEdit = new RepositoryItemPopupContainerEdit();
      private readonly RepositoryItemTextEdit _repositoryItemConstantParameter = new RepositoryItemTextEdit();
      private readonly PopupContainerControl _popupControl = new PopupContainerControl();
      private ICompoundParameterGroupPresenter _presenter;
      private IGridViewAutoBindColumn<IParameterDTO, double> _columnValue;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };
      protected readonly ComboBoxUnitParameter _comboBoxUnit;

      public MolWeightGroupView()
      {
         InitializeComponent();
         InitializeWithGrid(_gridView);
         _repositoryItemPopupContainerEdit.Buttons[0].Kind = ButtonPredefines.Combo;
         _repositoryItemPopupContainerEdit.PopupControl = _popupControl;
         _repositoryItemPopupContainerEdit.CloseOnOuterMouseClick = false;
         _repositoryItemPopupContainerEdit.QueryDisplayText += (o, e) => OnEvent(queryDisplayText, e);
         _repositoryItemPopupContainerEdit.EditValueChanged += (o, e) => OnEvent(() => _gridView.PostEditor());
         _repositoryItemPopupContainerEdit.QueryCloseUp += (o, e) => OnEvent(saveHalogens);
         _repositoryItemPopupContainerEdit.CloseUp += (o, e) => OnEvent(saveHalogens);

         _gridView.AllowsFiltering = false;
         _gridView.ShowColumnHeaders = false;
         _gridView.EditorShowMode = EditorShowMode.MouseUp;
         _gridView.ShowingEditor += onShowingEditor;
         _gridView.ShowRowIndicator = false;
         _gridView.RowStyle += rowStyle;
         _gridView.HiddenEditor += (o, e) => OnEvent(hideEditor);

         _comboBoxUnit = new ComboBoxUnitParameter(gridControl);

         _gridViewBinder = new GridViewBinder<IParameterDTO>(_gridView)
         {
            BindingMode = BindingMode.OneWay
         };
      }

      private void hideEditor()
      {
         _comboBoxUnit.Hide();
      }

      private void saveHalogens()
      {
         molWeightGroupPresenter.SaveHalogens();
      }

      private void queryDisplayText(QueryDisplayTextEventArgs e)
      {
         var hasHalogens = _gridViewBinder.FocusedElement;
         if (hasHalogens == null) return;
         e.DisplayText = hasHalogens.Value == 1 ? "Yes" : "No";
      }

      public void AttachPresenter(ICompoundParameterGroupPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(param => param.DisplayName)
            .WithCaption(PKSimConstants.UI.Name)
            .AsReadOnly();

         _columnValue = _gridViewBinder.AutoBind(param => param.Value)
            .WithFormat(getParameterFormatter)
            .WithCaption(PKSimConstants.UI.Value)
            .WithRepository(getRepository)
            .WithEditorConfiguration(configureRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => setParameterValue(o, e)));

         _comboBoxUnit.ParameterUnitSet += (o, e) => OnEvent(() => setParameterUnit(o, e));
      }

      private void setParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         _gridView.CloseEditor();
         molWeightGroupPresenter.SetParameterUnit(parameterDTO, newUnit);
      }

      protected override void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         var parameterDTO = _gridViewBinder.ElementAt(rowHandle);
         if (parameterDTO == null) return;

         _gridView.EditorShowMode = molWeightGroupPresenter.IsHasHalogens(parameterDTO) ? EditorShowMode.Default : EditorShowMode.MouseUp;
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_columnValue == null) return false;
         return _columnValue.XtraColumn == gridColumn;
      }

      private IFormatter<double> getParameterFormatter(IParameterDTO parameterDTO)
      {
         return molWeightGroupPresenter.IsHasHalogens(parameterDTO) ? new BooleanFormatter() : parameterDTO.ParameterFormatter();
      }

      private void setParameterValue(IParameterDTO parameterDTO, PropertyValueSetEventArgs<double> valueInGuiUnit)
      {
         molWeightGroupPresenter.SetParameterValue(parameterDTO, valueInGuiUnit.NewValue);
      }

      private void rowStyle(object sender, RowStyleEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e.RowHandle);
         if (parameterDTO == null) return;
         _gridView.AdjustAppearance(e, !molWeightGroupPresenter.IsMolWeightEff(parameterDTO));
      }

      private RepositoryItem getRepository(IParameterDTO parameterDTO)
      {
         return molWeightGroupPresenter.IsHasHalogens(parameterDTO) ? _repositoryItemPopupContainerEdit : _repositoryItemConstantParameter;
      }

      private void configureRepository(BaseEdit baseEdit, IParameterDTO parameterDTO)
      {
         if (molWeightGroupPresenter.IsHasHalogens(parameterDTO))
            OnEvent(molWeightGroupPresenter.EditHalogens);
         else
            _comboBoxUnit.UpdateUnitsFor(baseEdit, parameterDTO);
      }

      public void BindTo(IEnumerable<IParameterDTO> molWeightsDTO)
      {
         _gridViewBinder.BindToSource(molWeightsDTO.ToBindingList());
         AdjustHeight();
      }

      public override bool HasError => _gridViewBinder.HasError;

      public void SetHalogensView(IView view)
      {
         _popupControl.FillWith(view);
      }

      public void AddValueOriginView(IView view)
      {
         AddViewTo(layoutItemValueOrigin, view);
      }

      public void RefreshData()
      {
         gridControl.RefreshDataSource();
      }

      private IMolWeightGroupPresenter molWeightGroupPresenter => _presenter.DowncastTo<IMolWeightGroupPresenter>();

      public void AdjustHeight()
      {
         layoutItemMolWeight.AdjustControlHeight(_gridView.OptimalHeight);
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         _gridView.LayoutChanged();
      }

      public int OptimalHeight => layoutControlGroup.Height + layoutControl.Margin.All;

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         var parameterDTO = _gridViewBinder.FocusedElement;
         if (parameterDTO == null) return;
         e.Cancel = molWeightGroupPresenter.IsMolWeightEff(parameterDTO);
      }
   }
}