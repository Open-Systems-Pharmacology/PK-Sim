using System.Drawing;
using System.Linq;
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
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class CommitSimulationParametersView : BaseModalView, ICommitSimulationParametersView
   {
      private ICommitSimulationParametersPresenter _presenter;
      private readonly GridViewBinder<ParameterCommitDTO> _parameterGridBinder;
      private readonly ScreenBinder<CompoundCommitDTO> _screenBinder;
      private readonly UxRepositoryItemCheckEdit _parameterCheckEditor;
      private CompoundCommitDTO _dto;

      private const int CREATE_NEW = 0;
      private const int UPDATE_EXISTING = 1;

      public CommitSimulationParametersView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _parameterGridBinder = new GridViewBinder<ParameterCommitDTO>(gridViewParameters) { BindingMode = BindingMode.TwoWay };
         _parameterCheckEditor = new UxRepositoryItemCheckEdit(gridViewParameters);
         _screenBinder = new ScreenBinder<CompoundCommitDTO>();

         gridViewParameters.ShowRowIndicator = false;
         gridViewParameters.OptionsDetail.EnableMasterViewMode = false;
         gridViewParameters.OptionsView.ShowGroupPanel = false;
      }

      public void AttachPresenter(ICommitSimulationParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(CompoundCommitDTO dto)
      {
         _dto = dto;
         _parameterGridBinder.BindToSource(dto.Parameters);
         _screenBinder.BindToSource(dto);
         updateCommitOptionsFor(dto);
         SetOkButtonEnable();
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

         _screenBinder.Bind(x => x.NewSetName)
            .To(tbNewSetName);

         RegisterValidationFor(_screenBinder, statusChangedNotify: SetOkButtonEnable);

         radioGroupCommitMode.SelectedIndexChanged += (s, e) => OnEvent(commitModeChanged);
         cbExistingSet.SelectedIndexChanged += (s, e) => OnEvent(existingSetChanged);

         _parameterGridBinder.Changed += SetOkButtonEnable;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         ApplicationIcon = ApplicationIcons.Commit;
         MinimumSize = new Size(600, 380);
         ClientSize = new Size(700, 460);

         radioGroupCommitMode.Properties.AllowMouseWheel = false;
         cbExistingSet.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
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

      protected override bool IsOkButtonEnable =>
         base.IsOkButtonEnable && _dto != null && _dto.Parameters.Any(p => p.Selected);

      private void updateCommitOptionsFor(CompoundCommitDTO compound)
      {
         radioGroupCommitMode.EditValue = compound.CreateNew ? CREATE_NEW : UPDATE_EXISTING;

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
         if (_dto == null) return;

         var isCreateNew = _dto.CreateNew;
         layoutItemNewSetName.Visibility = toVisibility(isCreateNew);
         layoutItemExistingSet.Visibility = toVisibility(!isCreateNew && hasExistingSets);

         radioGroupCommitMode.Properties.Items[UPDATE_EXISTING].Enabled = hasExistingSets;
      }

      private bool hasExistingSets => _dto?.AvailableExistingSets != null && _dto.AvailableExistingSets.Any();

      private void commitModeChanged()
      {
         if (_dto == null) return;

         _dto.CreateNew = (int)radioGroupCommitMode.EditValue == CREATE_NEW;
         updateOptionsVisibility();
         // Re-validate since CreateNew change affects NewSetName validation
         _screenBinder.Validate();
         SetOkButtonEnable();
      }

      private void existingSetChanged()
      {
         if (_dto == null) return;

         var selectedName = cbExistingSet.SelectedItem as string;
         _dto.SelectedExistingSet = _dto.AvailableExistingSets?
            .FirstOrDefault(s => s.Name == selectedName);
      }

      private static DevExpress.XtraLayout.Utils.LayoutVisibility toVisibility(bool visible)
      {
         return visible
            ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
            : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
      }

      public override bool HasError => _screenBinder.HasError || _parameterGridBinder.HasError;
   }
}
