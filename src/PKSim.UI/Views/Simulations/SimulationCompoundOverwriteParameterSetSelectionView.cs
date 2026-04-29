using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundOverwriteParameterSetSelectionView : BaseGridViewOnlyUserControl, ISimulationCompoundOverwriteParameterSetSelectionView
   {
      private readonly GridViewBinder<SimulationCompoundOverwriteParameterSetSelectionDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _repositoryForOverwriteParameterSets;

      public SimulationCompoundOverwriteParameterSetSelectionView()
      {
         InitializeComponent();
         _repositoryForOverwriteParameterSets = new UxRepositoryItemComboBox(gridView);
         _gridViewBinder = new GridViewBinder<SimulationCompoundOverwriteParameterSetSelectionDTO>(gridView);
         gridView.HorzScrollVisibility = ScrollVisibility.Never;
         gridView.ShowColumnHeaders = false;
         gridView.RowStyle += rowStyle;
         layoutControl.AutoScroll = false;
         _repositoryForOverwriteParameterSets.AllowDropDownWhenReadOnly = DefaultBoolean.False;
      }

      public void AttachPresenter(ISimulationCompoundOverwriteParameterSetSelectionPresenter presenter)
      {
      }

      public void BindTo(SimulationCompoundOverwriteParameterSetSelectionDTO dto)
      {
         _gridViewBinder.BindToSource(new[] { dto }.ToBindingList());
         gridView.RefreshData();
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.ValidationMode = ValidationMode.LeavingRow;

         _gridViewBinder.Bind(x => x.Label)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.SelectedOverwriteParameterSet)
            .WithRepository(_ => _repositoryForOverwriteParameterSets)
            .WithEditorConfiguration(configureOverwriteParameterSets)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      public override bool HasError => _gridViewBinder.HasError;

      private void configureOverwriteParameterSets(BaseEdit activeEditor, SimulationCompoundOverwriteParameterSetSelectionDTO dto)
      {
         activeEditor.FillComboBoxEditorWith(dto.AllOverwriteParameterSets);
         activeEditor.Enabled = dto.AllOverwriteParameterSets.Count > 1;
      }

      private void rowStyle(object sender, RowStyleEventArgs e)
      {
         var dto = _gridViewBinder.ElementAt(e.RowHandle);
         if (dto == null) return;

         gridView.AdjustAppearance(e, dto.AllOverwriteParameterSets.Count > 1);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.OverwriteParameterSetSelection;
      }
   }
}
