using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Compounds;

public partial class OverwriteParameterSetsView : BaseUserControl, IOverwriteParameterSetsView
{
   private const int METADATA_EDITOR_WIDTH = 200;

   private IOverwriteParameterSetsPresenter _presenter;
   private readonly IImageListRetriever _imageListRetriever;
   private readonly GridViewBinder<OverwriteParameterSetDTO> _gridViewBinderSets;
   private readonly GridViewBinder<OverwriteParameterValueDTO> _gridViewBinderParameterValues;
   private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
   private readonly RepositoryItemButtonEdit _removeSetButtonRepository = new UxRemoveButtonRepository();
   private readonly UxRepositoryItemCheckEdit _isDefaultRepository;
   private readonly UxRepositoryItemImageComboBox _speciesRepository;
   private readonly UxRepositoryItemImageComboBox _diseaseStateRepository;
   private readonly ToolTipController _metadataToolTipController = new();
   private readonly List<BaseEdit> _metadataEditors = new();
   private LayoutControlGroup _metadataGroup;

   public OverwriteParameterSetsView(IImageListRetriever imageListRetriever)
   {
      _imageListRetriever = imageListRetriever;
      InitializeComponent();

      _gridViewBinderSets = new GridViewBinder<OverwriteParameterSetDTO>(gridViewSets)
      {
         BindingMode = BindingMode.OneWay
      };

      _gridViewBinderParameterValues = new GridViewBinder<OverwriteParameterValueDTO>(gridViewParameterValues)
      {
         BindingMode = BindingMode.OneWay
      };

      _isDefaultRepository = new UxRepositoryItemCheckEdit(gridViewSets);
      _speciesRepository = new UxRepositoryItemImageComboBox(gridViewSets, imageListRetriever);
      _diseaseStateRepository = new UxRepositoryItemImageComboBox(gridViewSets, imageListRetriever);
      
      // Disease state options use DisplayName for UI but no icon. Clearing the image lists hides the icon column.
      _diseaseStateRepository.SmallImages = null;
      _diseaseStateRepository.LargeImages = null;

      gridViewSets.OptionsSelection.EnableAppearanceFocusedCell = false;
      gridViewParameterValues.OptionsSelection.EnableAppearanceFocusedCell = false;
      gridViewParameterValues.EditorShowMode = EditorShowMode.MouseDown;
      gridViewSets.EditorShowMode = EditorShowMode.MouseDown;

      gridViewSets.FocusedRowChanged += (o, e) => OnEvent(selectedSetChanged);
   }

   public void AttachPresenter(IOverwriteParameterSetsPresenter presenter)
   {
      _presenter = presenter;
   }

   public override void InitializeBinding()
   {
      _gridViewBinderSets.Bind(x => x.Name)
         .WithCaption(PKSimConstants.UI.Name)
         .AsReadOnly();

      _gridViewBinderSets.Bind(x => x.IsDefault)
         .WithCaption(PKSimConstants.UI.IsDefault)
         .WithRepository(_ => _isDefaultRepository)
         .WithFixedWidth(EMBEDDED_CHECK_BOX_WIDTH)
         .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.SetIsDefault(dto, e.NewValue)));

      _gridViewBinderSets.Bind(x => x.Species)
         .WithCaption(PKSimConstants.UI.Species)
         .WithRepository(_ => configureSpeciesRepository())
         .WithShowButton(ShowButtonModeEnum.ShowAlways)
         .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.SetExtendedProperty(dto, PKSimConstants.UI.Species, e.NewValue)));

      _gridViewBinderSets.Bind(x => x.DiseaseState)
         .WithCaption(PKSimConstants.UI.DiseaseState)
         .WithRepository(_ => configureDiseaseStateRepository())
         .WithShowButton(ShowButtonModeEnum.ShowAlways)
         .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.SetExtendedProperty(dto, PKSimConstants.UI.DiseaseState, e.NewValue)));

      _gridViewBinderSets.AddUnboundColumn()
         .WithCaption(Captions.EmptyColumn)
         .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
         .WithShowButton(ShowButtonModeEnum.ShowAlways)
         .WithRepository(_ => _removeSetButtonRepository);

      _removeSetButtonRepository.ButtonClick += (_, _) => OnEvent(() => _presenter.RemoveSet(_gridViewBinderSets.FocusedElement));

      _gridViewBinderParameterValues.Bind(x => x.Path)
         .WithCaption(Captions.Diff.ObjectPath)
         .AsReadOnly();

      _gridViewBinderParameterValues.Bind(x => x.Value)
         .WithCaption(Captions.Value)
         .WithOnValueUpdating((dto, e) => OnEvent(() => onParameterValueUpdating(dto, e.NewValue)));

      _gridViewBinderParameterValues.Bind(x => x.Unit)
         .WithCaption(Captions.Unit)
         .AsReadOnly();

      _gridViewBinderParameterValues.Bind(x => x.ValueOrigin)
         .WithCaption(Captions.ValueOrigin)
         .AsReadOnly();

      _gridViewBinderParameterValues.AddUnboundColumn()
         .WithCaption(Captions.EmptyColumn)
         .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
         .WithShowButton(ShowButtonModeEnum.ShowAlways)
         .WithRepository(_ => _removeButtonRepository);

      _removeButtonRepository.ButtonClick += (_, _) => OnEvent(() => _presenter.RemoveParameterValue(_gridViewBinderSets.FocusedElement, _gridViewBinderParameterValues.FocusedElement));
   }

   public override void InitializeResources()
   {
      base.InitializeResources();
      Caption = PKSimConstants.UI.OverwriteParameterSetsTabCaption;
      ApplicationIcon = ApplicationIcons.ParameterValues;
   }

   public void BindTo(IReadOnlyList<OverwriteParameterSetDTO> overwriteParameterSets)
   {
      _gridViewBinderSets.BindToSource(overwriteParameterSets);
      bindToSelectedParameterValues();
   }

   private void selectedSetChanged() => bindToSelectedParameterValues();

   private void bindToSelectedParameterValues()
   {
      var selectedSet = _gridViewBinderSets.FocusedElement;
      buildMetadataPanel(selectedSet);
      _gridViewBinderParameterValues.BindToSource(selectedSet?.ParameterValues ?? []);
   }

   private void buildMetadataPanel(OverwriteParameterSetDTO selectedSet)
   {
      rightPanelLayoutControl.DoInBatch(() =>
      {
         disposeMetadataGroup();

         _metadataGroup = rightPanelLayoutControl.AddGroup();
         _metadataGroup.Text = PKSimConstants.UI.Metadata;
         _metadataGroup.LayoutMode = LayoutMode.Flow;
         _metadataGroup.Move(parameterValuesGroup, InsertType.Top);
         _metadataGroup.Enabled = selectedSet != null;

         metadataPropertyNamesFor(selectedSet).Each(x => addMetadataItem(selectedSet, x, valueOf(selectedSet, x)));
      });
   }

   private static IEnumerable<string> metadataPropertyNamesFor(OverwriteParameterSetDTO selectedSet)
   {
      var defaultPropertyNames = new List<string> { PKSimConstants.UI.Species, PKSimConstants.UI.DiseaseState };
      if (selectedSet == null)
         return defaultPropertyNames;

      var existingNames = selectedSet.OverwriteParameterSet.ExtendedProperties.All.Select(x => x.Name);

      // preserve this order (adding existing names to defaults). That ensures that Species and Disease State
      // are always created in this order and before any other extended properties
      defaultPropertyNames.AddRange(existingNames);
      return defaultPropertyNames.Distinct();
   }

   private static string valueOf(OverwriteParameterSetDTO selectedSet, string propertyName)
   {
      if (selectedSet == null)
         return string.Empty;

      var extendedProperties = selectedSet.OverwriteParameterSet.ExtendedProperties;
      if (!extendedProperties.Contains(propertyName))
         return string.Empty;

      return extendedProperties[propertyName].ValueAsObject?.ToString() ?? string.Empty;
   }

   private void addMetadataItem(OverwriteParameterSetDTO selectedSet, string propertyName, string value)
   {
      var editor = createMetadataEditor(selectedSet, propertyName, value);
      editor.Width = METADATA_EDITOR_WIDTH;
      editor.MinimumSize = editor.MinimumSize with { Width = METADATA_EDITOR_WIDTH };
      editor.MaximumSize = editor.MaximumSize with { Width = METADATA_EDITOR_WIDTH };

      var item = rightPanelLayoutControl.AddItem();
      item.Text = propertyName.FormatForLabel(checkCase: false);
      item.Control = editor;
      _metadataGroup.AddItem(item);
   }

   private BaseEdit createMetadataEditor(OverwriteParameterSetDTO selectedSet, string propertyName, string value)
   {
      var editor = createEditorFor(propertyName, value);
      editor.ToolTipController = _metadataToolTipController;
      editor.ToolTip = editor.Text;
      editor.EditValueChanged += (_, _) =>
      {
         OnEvent(() => _presenter.SetExtendedProperty(selectedSet, propertyName, editor.EditValue.ToString()));
         editor.ToolTip = editor.Text;
      };

      _metadataEditors.Add(editor);
      return editor;
   }

   private BaseEdit createEditorFor(string propertyName, string value)
   {
      if (string.Equals(propertyName, PKSimConstants.UI.Species))
         return createImageComboBoxEditor(_presenter.AllSpecies(), value, withIcons: true);

      if (string.Equals(propertyName, PKSimConstants.UI.DiseaseState))
         return createImageComboBoxEditor(_presenter.AllDiseaseStates(), value, withIcons: false);

      return new TextEdit { Text = value };
   }

   private ImageComboBoxEdit createImageComboBoxEditor(IReadOnlyList<ExtendedPropertyOptionDTO> options, string value, bool withIcons)
   {
      var comboBox = new UxImageComboBoxEdit();
      comboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
      if (withIcons)
         comboBox.Properties.SmallImages = _imageListRetriever.AllImages16x16;

      comboBox.Properties.Items.Add(new ImageComboBoxItem(string.Empty, string.Empty, -1));
      foreach (var option in options)
         comboBox.Properties.Items.Add(new ImageComboBoxItem(option.DisplayName, option.Name, withIcons ? _imageListRetriever.ImageIndex(option.Icon) : -1));

      comboBox.EditValue = value ?? string.Empty;
      return comboBox;
   }

   private void disposeMetadataGroup()
   {
      if (_metadataGroup == null)
         return;

      _metadataEditors.Each(x => x.Dispose());
      _metadataEditors.Clear();
      rightPanelLayoutControl.Remove(_metadataGroup);
      _metadataGroup.Dispose();
      _metadataGroup = null;
   }

   private RepositoryItem configureSpeciesRepository() =>
      fillImageComboBoxRepository(_speciesRepository, _presenter.AllSpecies(), withIcons: true);

   private RepositoryItem configureDiseaseStateRepository() =>
      fillImageComboBoxRepository(_diseaseStateRepository, _presenter.AllDiseaseStates(), withIcons: false);

   private RepositoryItem fillImageComboBoxRepository(UxRepositoryItemImageComboBox repository, IReadOnlyList<ExtendedPropertyOptionDTO> options, bool withIcons)
   {
      repository.Items.Clear();
      repository.Items.Add(new ImageComboBoxItem(string.Empty, string.Empty, -1));
      foreach (var option in options)
         repository.Items.Add(new ImageComboBoxItem(option.DisplayName, option.Name, withIcons ? _imageListRetriever.ImageIndex(option.Icon) : -1));

      return repository;
   }

   private void onParameterValueUpdating(OverwriteParameterValueDTO dto, double? newValue)
   {
      if (!newValue.HasValue)
         return;

      var selectedSet = _gridViewBinderSets.FocusedElement;
      if (selectedSet == null)
         return;

      _presenter.UpdateParameterValue(selectedSet, dto, newValue.Value);
      gridViewParameterValues.CloseEditor();
   }
}