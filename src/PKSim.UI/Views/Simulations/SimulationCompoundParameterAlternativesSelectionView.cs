using System.Collections.Generic;
using System.Linq;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundParameterAlternativesSelectionView : BaseGridViewOnlyUserControl, ISimulationCompoundParameterAlternativesSelectionView
   {
      private readonly GridViewBinder<CompoundParameterSelectionDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _repositoryForParameterAlternatives;
      private readonly RepositoryItemTextEdit _repositoryItemDisabled;
      private ISimulationCompoundParameterAlternativesSelectionPresenter _presenter;

      public SimulationCompoundParameterAlternativesSelectionView()
      {
         InitializeComponent();
         _repositoryForParameterAlternatives = new UxRepositoryItemComboBox(gridView);
         _repositoryItemDisabled = new RepositoryItemTextEdit {Enabled = false, ReadOnly = true};
         _gridViewBinder = new GridViewBinder<CompoundParameterSelectionDTO>(gridView);
         gridView.RowStyle += rowStyle;
         gridView.HorzScrollVisibility=ScrollVisibility.Never;
         layoutControl.AutoScroll = false;
         _repositoryForParameterAlternatives.AllowDropDownWhenReadOnly = DefaultBoolean.False;
      }

      public void AttachPresenter(ISimulationCompoundParameterAlternativesSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<CompoundParameterSelectionDTO> compoundParameterMappingDTO)
      {
         _gridViewBinder.BindToSource(compoundParameterMappingDTO.ToBindingList());
         gridView.RefreshData();
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.ValidationMode = ValidationMode.LeavingRow;
         _gridViewBinder.Bind(x => x.ParameterName)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.SelectedAlternative)
            .WithRepository(repositoryForParameterAlternative)
            .WithEditorConfiguration(configureParameterAlternatives)
            .WithCaption(PKSimConstants.UI.AlternativeInCompound)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      public override bool HasError => _gridViewBinder.HasError;

      private void rowStyle(object sender, RowStyleEventArgs e)
      {
         var compoundParameterSelectionDTO = _gridViewBinder.ElementAt(e.RowHandle);
         if (compoundParameterSelectionDTO == null) return;

         var allAlternatives = _presenter.AllAlternativesFor(compoundParameterSelectionDTO);
         gridView.AdjustAppearance(e, allAlternatives.Count() > 1);
      }

      private RepositoryItem repositoryForParameterAlternative(CompoundParameterSelectionDTO compoundParameterSelectionDTO)
      {
         var allAlternatives = _presenter.AllAlternativesFor(compoundParameterSelectionDTO).ToList();
         if (allAlternatives.Count > 1)
            return _repositoryForParameterAlternatives;

         return _repositoryItemDisabled;
      }

      private void configureParameterAlternatives(BaseEdit activeEditor, CompoundParameterSelectionDTO compoundParameterSelectionDTO)
      {
         var allAlternatives = _presenter.AllAlternativesFor(compoundParameterSelectionDTO).ToList();
         if (allAlternatives.Count > 1)
            activeEditor.FillComboBoxEditorWith(allAlternatives);
         else
            activeEditor.Enabled = false;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.ParameterAlternatives;
      }
   }
}