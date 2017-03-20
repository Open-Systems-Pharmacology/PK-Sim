using System;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class SolubilityGroupView : CompoundParameterWithDefaultAlternativeBaseView<SolubilityAlternativeDTO>, ISolubilityGroupView
   {
      private IGridViewColumn _colSolubitlity;
      private IGridViewColumn _colRefPh;
      private IGridViewColumn _colGainPerCharge;

      private readonly PopupContainerControl _popupControl = new PopupContainerControl();
      private readonly RepositoryItemPopupContainerEdit _repositoryItemPopupContainerEdit = new RepositoryItemPopupContainerEdit();

      public SolubilityGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever) : base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
         _repositoryItemPopupContainerEdit.Buttons[0].Kind = ButtonPredefines.Combo;
         _repositoryItemPopupContainerEdit.PopupControl = _popupControl;
         _repositoryItemPopupContainerEdit.CloseOnOuterMouseClick = false;
         _repositoryItemPopupContainerEdit.QueryDisplayText += queryDisplayText;
         _repositoryItemPopupContainerEdit.EditValueChanged += editValueChanged;
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
            .WithEditorConfiguration((activeEditor, sol) => _comboBoxUnit.UpdateUnitsFor(activeEditor, sol.SolubilityParameter))
            .WithOnValueSet((dto, e) => OnEvent(() => solubilityGroupPresenter.SetSolubilityValue(dto, e.NewValue)));

         _comboBoxUnit.ParameterUnitSet += (dto, unit) => OnEvent(() => solubilityGroupPresenter.SetSolubilityUnit(dto, unit));

         _colRefPh = _gridViewBinder.AutoBind(x => x.RefpH)
            .WithCaption(PKSimConstants.UI.RefpH)
            .WithOnValueSet((dto, e) => OnEvent(() => solubilityGroupPresenter.SetRefpHValue(dto, e.NewValue)));

         _colGainPerCharge = _gridViewBinder.AutoBind(x => x.GainPerCharge)
            .WithCaption(PKSimConstants.UI.SolubilityGainPerCharge)
            .WithOnValueSet((dto, e) => OnEvent(() => solubilityGroupPresenter.SetGainPerChanrgeValue(dto, e.NewValue)));

         var col = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.pHDependentSolubility)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => _repositoryItemPopupContainerEdit)
            .WithEditorConfiguration(updateChart);

         col.XtraColumn.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;

         //to do at the end to respect order
         base.InitializeBinding();
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

      public void SetChartView(IView view)
      {
         _popupControl.FillWith(view);
      }

      private ISolubilityGroupPresenter solubilityGroupPresenter => _presenter.DowncastTo<ISolubilityGroupPresenter>();
   }
}