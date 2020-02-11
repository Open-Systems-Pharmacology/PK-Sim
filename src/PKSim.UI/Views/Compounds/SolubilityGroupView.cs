using System;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   public partial class SolubilityGroupView : CompoundParameterWithDefaultAlternativeBaseView<SolubilityAlternativeDTO>, ISolubilityGroupView
   {
      private IGridViewColumn _colSolubitlity;
      private IGridViewColumn _colRefPh;
      private IGridViewColumn _colGainPerCharge;

      private readonly PopupContainerControl _popupControl = new PopupContainerControl();
      private readonly RepositoryItemPopupContainerEdit _repositoryItemPopupContainerEdit = new RepositoryItemPopupContainerEdit();
      private readonly RepositoryItemTextEdit _stantdardParameterEditRepository = new RepositoryItemTextEdit();
      private readonly RepositoryItemTextEdit _readonlyTextEdit = new RepositoryItemTextEdit();
      private readonly UxRepositoryItemButtonEdit _editTableParameterRepository = new UxRepositoryItemButtonEdit(ButtonPredefines.Glyph);

      public SolubilityGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, ValueOriginBinder<SolubilityAlternativeDTO> valueOriginBinder) :
         base(toolTipCreator, imageListRetriever, valueOriginBinder)
      {
         InitializeComponent();
         _repositoryItemPopupContainerEdit.Buttons[0].Kind = ButtonPredefines.Combo;
         _repositoryItemPopupContainerEdit.PopupControl = _popupControl;
         _repositoryItemPopupContainerEdit.CloseOnOuterMouseClick = false;
         _repositoryItemPopupContainerEdit.QueryDisplayText += queryDisplayText;
         _repositoryItemPopupContainerEdit.EditValueChanged += editValueChanged;
         _stantdardParameterEditRepository.ConfigureWith(typeof(double));
         _stantdardParameterEditRepository.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
         _readonlyTextEdit.ReadOnly = true;
         _gridView.RowCellStyle += (o, e) => OnEvent(updateRowCellStyle, e);
         _gridView.ShowingEditor += (o, e) => OnEvent(onShowingEditor, e);
      }

      private void editValueChanged(object sender, EventArgs e)
      {
         _gridView.PostEditor();
      }

      private void queryDisplayText(object sender, QueryDisplayTextEventArgs e)
      {
         e.DisplayText = PKSimConstants.UI.ShowSolubilityPhChart;
      }

      public override void InitializeBinding()
      {
         _colSolubitlity = _gridViewBinder.AutoBind(x => x.Solubility)
            .WithCaption(PKSimConstants.UI.RefSolubility)
            .WithFormat(param => param.SolubilityParameter.ParameterFormatter())
            .WithRepository(repoForSolubility)
            .WithEditorConfiguration(configureRepository)
            .WithOnValueUpdating((dto, e) => OnEvent(() => solubilityGroupPresenter.SetSolubilityValue(dto, e.NewValue)));

         _comboBoxUnit.ParameterUnitSet += (dto, unit) => OnEvent(() => solubilityGroupPresenter.SetSolubilityUnit(dto, unit));

         _colRefPh = _gridViewBinder.AutoBind(x => x.RefpH)
            .WithFormat(secondaryParameterFormatter)
            .WithRepository(repoForSecondarySolubilityParameters)
            .WithCaption(PKSimConstants.UI.RefpH)
            .WithOnValueUpdating((dto, e) => OnEvent(() => solubilityGroupPresenter.SetRefpHValue(dto, e.NewValue)));

         _colGainPerCharge = _gridViewBinder.AutoBind(x => x.GainPerCharge)
            .WithFormat(secondaryParameterFormatter)
            .WithRepository(repoForSecondarySolubilityParameters)
            .WithCaption(PKSimConstants.UI.SolubilityGainPerCharge)
            .WithOnValueUpdating((dto, e) => OnEvent(() => solubilityGroupPresenter.SetGainPerChargeValue(dto, e.NewValue)));

         var col = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.pHDependentSolubility)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => _repositoryItemPopupContainerEdit)
            .WithEditorConfiguration(updateChart);

         col.XtraColumn.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;


         _editTableParameterRepository.ButtonClick += (o, e) => OnEvent(editSolubilityTable);

         //to do at the end to respect order
         base.InitializeBinding();
      }

      private IFormatter<double> secondaryParameterFormatter(SolubilityAlternativeDTO solubilityAlternative) => new SolubilityParameterFormatter(solubilityAlternative);

      private void updateRowCellStyle(RowCellStyleEventArgs e)
      {
         _gridView.AdjustAppearance(e, !isTableSecondaryParameter(e.Column, _gridViewBinder.ElementAt(e.RowHandle)));
      }

      private void onShowingEditor(CancelEventArgs e)
      {
         e.Cancel = isTableSecondaryParameter(_gridView.FocusedColumn, _gridViewBinder.FocusedElement);
      }

      private bool isTableSecondaryParameter(GridColumn column, SolubilityAlternativeDTO solubilityAlternative)
      {
         if (!column.IsOneOf(_colRefPh.XtraColumn, _colGainPerCharge.XtraColumn))
            return false;

         if (solubilityAlternative == null)
            return false;

         return solubilityAlternative.IsTable;
      }

      private RepositoryItem repoForSolubility(SolubilityAlternativeDTO solubilityAlternative)
      {
         if (solubilityAlternative.IsTable)
            return _editTableParameterRepository;

         return _stantdardParameterEditRepository;
      }

      private RepositoryItem repoForSecondarySolubilityParameters(SolubilityAlternativeDTO solubilityAlternative)
      {
         if (solubilityAlternative.IsTable)
            return _readonlyTextEdit;

         return _stantdardParameterEditRepository;
      }

      private void editSolubilityTable()
      {
         solubilityGroupPresenter.EditSolubilityTable(_gridViewBinder.FocusedElement);
      }

      private void configureRepository(BaseEdit activeEditor, SolubilityAlternativeDTO solubilityAlternative)
      {
         if (solubilityAlternative.IsTable)
            return;

         _comboBoxUnit.UpdateUnitsFor(activeEditor, solubilityAlternative.SolubilityParameter);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         _editTableParameterRepository.Buttons[0].Caption = PKSimConstants.UI.EditTable;
      }

      private void updateChart(BaseEdit baseEdit, SolubilityAlternativeDTO solubilityAlternative)
      {
         OnEvent(() => solubilityGroupPresenter.UpdateSolubilityChart(solubilityAlternative));
      }

      private void clearPopupContainer()
      {
         _repositoryItemPopupContainerEdit.QueryDisplayText -= queryDisplayText;
         _repositoryItemPopupContainerEdit.EditValueChanged -= editValueChanged;
         _repositoryItemPopupContainerEdit.PopupControl = null;
         _popupControl.Dispose();
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colSolubitlity == null || _colRefPh == null || _colGainPerCharge == null)
            return false;

         return gridColumn.IsOneOf(_colRefPh.XtraColumn, _colSolubitlity.XtraColumn, _colGainPerCharge.XtraColumn);
      }

      protected override void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         var solubilityAlternative = _gridViewBinder.ElementAt(rowHandle);
         if (solubilityAlternative == null) return;

         _gridView.EditorShowMode = solubilityAlternative.IsTable ? EditorShowMode.Default : EditorShowMode.MouseUp;
      }

      public void SetChartView(IView view)
      {
         _popupControl.FillWith(view);
      }

      private ISolubilityGroupPresenter solubilityGroupPresenter => _presenter.DowncastTo<ISolubilityGroupPresenter>();
   }
}