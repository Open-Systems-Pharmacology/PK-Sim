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
using PKSim.Presentation.Services;
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
      private const int UPDATE_SELECTED = 1;

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
         bindVisibleParameters();
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

         _parameterGridBinder.Bind(x => x.Change)
            .WithCaption(PKSimConstants.UI.Change)
            .WithFixedWidth(120)
            .AsReadOnly();

         _parameterGridBinder.Bind(x => x.Value)
            .WithCaption(PKSimConstants.UI.Value)
            .WithFormat(dto => new ValueWithUnitFormatter<double>(() => dto.Unit))
            .WithFixedWidth(120)
            .AsReadOnly();

         _parameterGridBinder.Bind(x => x.ValueOrigin)
            .WithCaption(Captions.ValueOrigin)
            .AsReadOnly();

         _screenBinder.Bind(x => x.NewSetName)
            .To(tbNewSetName);

         RegisterValidationFor(_screenBinder, statusChangedNotify: SetOkButtonEnable);

         radioGroupCommitMode.SelectedIndexChanged += (s, e) => OnEvent(commitModeChanged);

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
         radioGroupCommitMode.Properties.Items.AddRange([
            new RadioGroupItem(CREATE_NEW, PKSimConstants.Command.CreateNewParameterSet),
            new RadioGroupItem(UPDATE_SELECTED, PKSimConstants.Command.UpdateParameterSet)
         ]);

         layoutItemNewSetName.Text = PKSimConstants.UI.Name.FormatForLabel();
         layoutGroupOptions.Text = PKSimConstants.Command.CommitOptions;

         layoutItemCommitMode.AdjustControlHeight(54);
         layoutItemNewSetName.AdjustControlHeight(24);
      }

      protected override bool IsOkButtonEnable =>
         base.IsOkButtonEnable && _dto != null && _dto.VisibleParameters.Any(p => p.Selected);

      private void bindVisibleParameters() => _parameterGridBinder.BindToSource(_dto.VisibleParameters.ToList());

      private void updateCommitOptionsFor(CompoundCommitDTO compound)
      {
         radioGroupCommitMode.Properties.Items[CREATE_NEW].Enabled = compound.CanCreateNewSet;

         var updateItem = radioGroupCommitMode.Properties.Items[UPDATE_SELECTED];
         updateItem.Enabled = compound.CanUpdateSetSelectedInSimulation;
         updateItem.Description = compound.CanUpdateSetSelectedInSimulation
            ? PKSimConstants.Command.UpdateParameterSetNamed(compound.SetSelectedInSimulation.Name)
            : PKSimConstants.Command.UpdateParameterSet;
         radioGroupCommitMode.ToolTip = compound.CanUpdateSetSelectedInSimulation
            ? string.Empty
            : PKSimConstants.Error.NoOverwriteParameterSetSelectedForCompoundInSimulation(compound.CompoundName);

         radioGroupCommitMode.EditValue = compound.CreateNew ? CREATE_NEW : UPDATE_SELECTED;
         updateOptionsVisibility();
      }

      private void updateOptionsVisibility()
      {
         if (_dto == null) return;

         layoutItemNewSetName.Visibility = toVisibility(_dto.CreateNew);
      }

      private void commitModeChanged()
      {
         if (_dto == null) return;

         _dto.CreateNew = (int)radioGroupCommitMode.EditValue == CREATE_NEW;
         bindVisibleParameters();
         updateOptionsVisibility();
         // Re-validate since CreateNew change affects NewSetName validation
         _screenBinder.Validate();
         SetOkButtonEnable();
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
