using System;
using System.ComponentModel;
using System.Linq.Expressions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Assets;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using PKSim.UI.Extensions;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Populations
{
   public partial class RandomPopulationSettingsView : BaseUserControl, IRandomPopulationSettingsView
   {
      private readonly GridViewBinder<ParameterRangeDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _repositoryForUnits;
      private readonly UxRepositoryItemComboBox _repositoryForDiscreteUnits;
      private readonly RepositoryItemTextEdit _repositoryFromItem = new RepositoryItemTextEdit {NullText = "from"};
      private readonly RepositoryItemTextEdit _repositoryToItem = new RepositoryItemTextEdit {NullText = "to"};
      private readonly RepositoryItemTextEdit _standardParameterEditRepository = new RepositoryItemTextEdit();
      private readonly ScreenBinder<PopulationSettingsDTO> _settingsBinder;
      private readonly UxBuildingBlockSelection _uxIndividualSelection;
      private readonly UxRepositoryItemComboBox _discreteParameterRepository;

      private IGridViewColumn _colFrom;
      private IGridViewColumn _colTo;
      private IRandomPopulationSettingsPresenter _presenter;
      private IGridViewColumn _colUnit;

      public RandomPopulationSettingsView()
      {
         InitializeComponent();
         gridViewParameters.AllowsFiltering = false;
         _gridViewBinder = new GridViewBinder<ParameterRangeDTO>(gridViewParameters)
         {
            ValidationMode = ValidationMode.LeavingCell,
            BindingMode = BindingMode.TwoWay
         };

         _settingsBinder = new ScreenBinder<PopulationSettingsDTO>();
         _uxIndividualSelection = new UxBuildingBlockSelection();
         _repositoryForUnits = new UxRepositoryItemComboBox(gridViewParameters);
         _discreteParameterRepository = new UxRepositoryItemComboBox(gridViewParameters);
         _repositoryForDiscreteUnits = new UxRepositoryItemComboBox(gridViewParameters) {AllowDropDownWhenReadOnly = DefaultBoolean.False, ReadOnly = true};
         _repositoryForDiscreteUnits.Buttons.Clear();

         gridViewParameters.RowCellStyle += updateRowCellStyle;
         gridViewParameters.ShowColumnHeaders = false;

         layoutItemIndividual.FillWith(_uxIndividualSelection);
      }

      public override void InitializeBinding()
      {
         gridViewParameters.ShowingEditor += onShowingEditor;

         _settingsBinder.Bind(x => x.Population)
            .To(lblPopulation);

         _settingsBinder.Bind(x => x.DiseaseState)
            .To(lblDiseaseState);
         
         _settingsBinder.Bind(x => x.Individual)
            .To(_uxIndividualSelection)
            .OnValueUpdating += (o, e) => _presenter.IndividualSelectionChanged(e.NewValue);

         _settingsBinder.Bind(dto => dto.NumberOfIndividuals)
            .To(tbNumberOfIndividuals);

         _settingsBinder.Bind(dto => dto.ProportionOfFemales)
            .To(tbProportionsOfFemales);

         RegisterValidationFor(_settingsBinder, NotifyViewChanged);

         _gridViewBinder.Bind(x => x.ParameterDisplayName)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();

         _colFrom = _gridViewBinder.AddUnboundColumn()
            .WithCaption(OSPSuite.UI.UIConstants.EMPTY_COLUMN)
            .WithRepository(fromRepository).AsReadOnly();
         _colFrom.XtraColumn.MaxWidth = 40;

         bindValue(x => x.MinValueInDisplayUnit);

         _colTo = _gridViewBinder.AddUnboundColumn()
            .WithCaption(OSPSuite.UI.UIConstants.EMPTY_COLUMN)
            .WithRepository(toRepository).AsReadOnly();

         _colTo.XtraColumn.MaxWidth = 40;

         bindValue(x => x.MaxValueInDisplayUnit);

         _colUnit = _gridViewBinder.AutoBind(x => x.Unit)
            .WithRepository(getUnitRepository)
            .WithEditorConfiguration(updateUnits)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.Changed += NotifyViewChanged;

         btnStop.Click += (o, e) => OnEvent(_presenter.Cancel);
      }

      private RepositoryItem getUnitRepository(ParameterRangeDTO parameterRangeDTO)
      {
         if (!parameterRangeDTO.IsDiscrete)
            return _repositoryForUnits;

         return _repositoryForDiscreteUnits;
      }

      private void bindValue(Expression<Func<ParameterRangeDTO, double?>> expression)
      {
         _gridViewBinder.Bind(expression)
            .WithRepository(repoForParameter)
            .WithEditorConfiguration(configureRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      private void configureRepository(BaseEdit activeEditor, ParameterRangeDTO parameter)
      {
         if (!parameter.IsDiscrete)
            return;

         activeEditor.FillComboBoxEditorWith(parameter.ListOfValues);
      }

      private RepositoryItem repoForParameter(ParameterRangeDTO parameterRangeDTO)
      {
         if (parameterRangeDTO.IsDiscrete)
            return _discreteParameterRepository;

         return _standardParameterEditRepository;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Population;

      public void BindTo(PopulationSettingsDTO populationSettingsDTO)
      {
         _settingsBinder.BindToSource(populationSettingsDTO);
         _gridViewBinder.BindToSource(populationSettingsDTO.Parameters);
         lblPopulation.Text = $"{PKSimConstants.UI.Population.FormatForLabel()} {populationSettingsDTO.Population}";
         var hasDiseaseState = populationSettingsDTO.DiseaseState.StringIsNotEmpty();
         layoutItemDiseaseState.Visibility = LayoutVisibilityConvertor.FromBoolean(hasDiseaseState);
         GenderSelectionVisible = populationSettingsDTO.HasMultipleGenders;

         adjustHeights();
         NotifyViewChanged();
      }

      public bool CreatingPopulation
      {
         set
         {
            layoutGroupParameterRanges.Enabled = !value;
            layoutGroupPopulationProperties.Enabled = layoutGroupParameterRanges.Enabled;
            layoutGroupIndividualSelection.Enabled = layoutGroupParameterRanges.Enabled;
            layoutItemStop.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         }
         get => !layoutGroupParameterRanges.Enabled;
      }

      public bool GenderSelectionVisible
      {
         set => layoutItemProportionOfFemales.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemProportionOfFemales.Visibility);
      }

      public void AttachPresenter(IRandomPopulationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void UpdateLayoutForEditing()
      {
         CreatingPopulation = true;
         var visibility = LayoutVisibilityConvertor.FromBoolean(false);
         layoutItemIndividual.Visibility = visibility;
         layoutItemDescription.Visibility = visibility;
         layoutItemPopulation.Visibility = visibility;
         layoutItemStop.Visibility = visibility;
         emptySpaceItem.Visibility = visibility;
      }


      public override bool HasError => _settingsBinder.HasError || _gridViewBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemIndividual.Text = PKSimConstants.UI.BasedOnIndividual.FormatForLabel();
         layoutItemProportionOfFemales.Text = PKSimConstants.UI.ProportionOfFemales.FormatForLabel();
         layoutItemNumberOfIndividuals.Text = PKSimConstants.UI.NumberOfIndividuals.FormatForLabel();
         layoutGroupPopulationProperties.Text = PKSimConstants.UI.PopulationProperties;
         layoutGroupParameterRanges.Text = PKSimConstants.UI.PopulationParameterRanges;
         lblDescription.Text = PKSimConstants.UI.IndividualIsMeanOfPopulation;
         lblDescription.AsDescription();
         Caption = PKSimConstants.UI.Demographics;
         layoutControl.InitializeDisabledColors();
         layoutItemStop.AdjustButtonSize();
         btnStop.InitWithImage(ApplicationIcons.Stop, PKSimConstants.UI.Stop);
         layoutItemStop.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
         layoutItemDiseaseState.TextVisible = true;
         layoutItemDiseaseState.Text = PKSimConstants.UI.DiseaseState.FormatForLabel();
         layoutItemPopulation.TextVisible = true;
         layoutItemPopulation.Text = PKSimConstants.UI.Population.FormatForLabel();
      }

      private void updateRowCellStyle(object sender, RowCellStyleEventArgs e)
      {
         if (e.Column == _colTo.XtraColumn || e.Column == _colFrom.XtraColumn)
         {
            gridViewParameters.AdjustAppearance(e, false);
         }
         if (e.Column == _colUnit.XtraColumn)
         {
            var element = _gridViewBinder.ElementAt(e.RowHandle);
            if (element == null) return;
            if (element.IsDiscrete)
               gridViewParameters.AdjustAppearance(e, false);
         }
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         //unit cannot be edited for discrete parameters;
         var parameterRange = _gridViewBinder.FocusedElement;
         if (parameterRange == null) return;
         if (gridViewParameters.FocusedColumn != _colUnit.XtraColumn) return;
         e.Cancel = parameterRange.IsDiscrete;
      }

      private void updateUnits(BaseEdit activeEditor, ParameterRangeDTO parameterRange)
      {
         activeEditor.FillComboBoxEditorWith(parameterRange.ParameterRange.Dimension.VisibleUnits());
         activeEditor.Enabled = !parameterRange.IsDiscrete;
      }

      private RepositoryItem toRepository(ParameterRangeDTO parameterRange) => _repositoryToItem;

      private RepositoryItem fromRepository(ParameterRangeDTO parameterRange) => _repositoryFromItem;

      private void adjustHeights()
      {
         layoutItemParameters.AdjustControlHeight(gridViewParameters.OptimalHeight);
      }
   }
}