using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class CommitSimulationParametersView : BaseModalView, ICommitSimulationParametersView
   {
      private ICommitSimulationParametersPresenter _presenter;
      private readonly GridViewBinder<ParameterCommitDTO> _parameterGridBinder;
      private readonly UxRepositoryItemCheckEdit _parameterCheckEditor;
      private List<CompoundCommitDTO> _compounds;
      private CompoundCommitDTO _selectedCompound;

      private const int CREATE_NEW = 0;
      private const int UPDATE_EXISTING = 1;

      public CommitSimulationParametersView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _parameterGridBinder = new GridViewBinder<ParameterCommitDTO>(gridViewParameters) { BindingMode = BindingMode.TwoWay };
         _parameterCheckEditor = new UxRepositoryItemCheckEdit(gridViewParameters);

         gridViewParameters.ShowRowIndicator = false;
         gridViewParameters.OptionsDetail.EnableMasterViewMode = false;
         gridViewParameters.OptionsView.ShowGroupPanel = false;
      }

      public void AttachPresenter(ICommitSimulationParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(CommitSimulationParametersDTO dto)
      {
         _compounds = dto.Compounds;

         listCompounds.Items.Clear();
         foreach (var compound in _compounds)
         {
            listCompounds.Items.Add(compound.CompoundName, compound.Selected ? CheckState.Checked : CheckState.Unchecked);
         }

         if (_compounds.Any())
         {
            listCompounds.SelectedIndex = 0;
            selectCompound(_compounds.First());
         }

         updateOkButton();
      }

      public override void InitializeBinding()
      {
         _parameterGridBinder.Bind(x => x.Selected)
            .WithRepository(x => _parameterCheckEditor)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH);

         _parameterGridBinder.Bind(x => x.DisplayPath)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();

         _parameterGridBinder.Bind(x => x.Value)
            .WithCaption(PKSimConstants.UI.Value)
            .WithFixedWidth(120)
            .AsReadOnly();

         listCompounds.SelectedIndexChanged += (s, e) => OnEvent(compoundSelectionChanged);
         listCompounds.ItemCheck += (s, e) => OnEvent(() => compoundCheckChanged(e));

         radioGroupCommitMode.SelectedIndexChanged += (s, e) => OnEvent(commitModeChanged);
         tbNewSetName.EditValueChanged += (s, e) => OnEvent(newSetNameChanged);
         cbExistingSet.SelectedIndexChanged += (s, e) => OnEvent(existingSetChanged);

         _parameterGridBinder.Changed += NotifyViewChanged;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         ApplicationIcon = ApplicationIcons.Commit;
         MinimumSize = new Size(550, 400);
         ClientSize = new Size(700, 480);

         listCompounds.Padding = new Padding(4);
         listCompounds.ItemHeight = 24;

         radioGroupCommitMode.Properties.AllowMouseWheel = false;
         radioGroupCommitMode.Properties.Items.AddRange([
            new RadioGroupItem(CREATE_NEW, PKSimConstants.Command.CreateNewParameterSet),
            new RadioGroupItem(UPDATE_EXISTING, PKSimConstants.Command.UpdateExistingParameterSet)
         ]);

         layoutItemNewSetName.Text = PKSimConstants.UI.Name.FormatForLabel();
         layoutItemExistingSet.Text = PKSimConstants.Command.ParameterSet.FormatForLabel();
         layoutGroupOptions.Text = PKSimConstants.Command.CommitOptions;

         layoutItemCommitMode.AdjustControlHeight(54);
         layoutItemNewSetName.AdjustControlHeight(24);
         layoutItemExistingSet.AdjustControlHeight(24);
      }

      private void compoundSelectionChanged()
      {
         if (listCompounds.SelectedIndex < 0 || _compounds == null)
            return;

         selectCompound(_compounds[listCompounds.SelectedIndex]);
      }

      private void compoundCheckChanged(DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
      {
         if (_compounds == null || e.Index >= _compounds.Count)
            return;

         _compounds[e.Index].Selected = e.State == CheckState.Checked;
         updateOkButton();
      }

      private void selectCompound(CompoundCommitDTO compound)
      {
         _selectedCompound = compound;
         _parameterGridBinder.BindToSource(compound.Parameters);
         updateCommitOptionsFor(compound);
      }

      private void updateCommitOptionsFor(CompoundCommitDTO compound)
      {
         radioGroupCommitMode.EditValue = compound.CreateNew ? CREATE_NEW : UPDATE_EXISTING;
         tbNewSetName.Text = compound.NewSetName;

         cbExistingSet.Properties.Items.Clear();
         if (compound.AvailableExistingSets != null && compound.AvailableExistingSets.Any())
         {
            foreach (var set in compound.AvailableExistingSets)
            {
               cbExistingSet.Properties.Items.Add(set.Name);
            }

            var selectedSet = compound.SelectedExistingSet ?? compound.AvailableExistingSets.FirstOrDefault();
            compound.SelectedExistingSet = selectedSet;
            cbExistingSet.SelectedItem = selectedSet?.Name;
         }

         updateOptionsVisibility();
      }

      private void updateOptionsVisibility()
      {
         if (_selectedCompound == null) return;

         var isCreateNew = _selectedCompound.CreateNew;
         layoutItemNewSetName.Visibility = toVisibility(isCreateNew);
         layoutItemExistingSet.Visibility = toVisibility(!isCreateNew && hasExistingSets);

         radioGroupCommitMode.Properties.Items[UPDATE_EXISTING].Enabled = hasExistingSets;
      }

      private bool hasExistingSets => _selectedCompound?.AvailableExistingSets != null && _selectedCompound.AvailableExistingSets.Any();

      private void commitModeChanged()
      {
         if (_selectedCompound == null) return;

         _selectedCompound.CreateNew = (int)radioGroupCommitMode.EditValue == CREATE_NEW;
         updateOptionsVisibility();
      }

      private void newSetNameChanged()
      {
         if (_selectedCompound == null) return;
         _selectedCompound.NewSetName = tbNewSetName.Text;
      }

      private void existingSetChanged()
      {
         if (_selectedCompound == null) return;

         var selectedName = cbExistingSet.SelectedItem as string;
         _selectedCompound.SelectedExistingSet = _selectedCompound.AvailableExistingSets?
            .FirstOrDefault(s => s.Name == selectedName);
      }

      private void updateOkButton()
      {
         OkEnabled = _compounds != null && _compounds.Any(c => c.Selected);
      }

      private static DevExpress.XtraLayout.Utils.LayoutVisibility toVisibility(bool visible)
      {
         return visible
            ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
      }

      public override bool HasError => _parameterGridBinder.HasError;
   }
}
