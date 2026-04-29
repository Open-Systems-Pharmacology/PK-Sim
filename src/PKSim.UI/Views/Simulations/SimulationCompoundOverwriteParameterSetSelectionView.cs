using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
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
      private readonly RepositoryItemTextEdit _repositoryItemDisabled;

      public SimulationCompoundOverwriteParameterSetSelectionView()
      {
         InitializeComponent();
         _repositoryForOverwriteParameterSets = new UxRepositoryItemComboBox(gridView);
         _repositoryItemDisabled = new RepositoryItemTextEdit { Enabled = false, ReadOnly = true };
         _gridViewBinder = new GridViewBinder<SimulationCompoundOverwriteParameterSetSelectionDTO>(gridView);
         gridView.HorzScrollVisibility = ScrollVisibility.Never;
         gridView.ShowColumnHeaders = false;
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
            .WithRepository(repositoryForOverwriteParameterSet)
            .WithEditorConfiguration(configureOverwriteParameterSets)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      public override bool HasError => _gridViewBinder.HasError;

      private RepositoryItem repositoryForOverwriteParameterSet(SimulationCompoundOverwriteParameterSetSelectionDTO dto)
      {
         if (dto.AllOverwriteParameterSets.Count > 1)
            return _repositoryForOverwriteParameterSets;

         return _repositoryItemDisabled;
      }

      private void configureOverwriteParameterSets(BaseEdit activeEditor, SimulationCompoundOverwriteParameterSetSelectionDTO dto)
      {
         if (dto.AllOverwriteParameterSets.Count > 1)
            activeEditor.FillComboBoxEditorWith(dto.AllOverwriteParameterSets);
         else
            activeEditor.Enabled = false;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.OverwriteParameterSetSelection;
      }
   }
}