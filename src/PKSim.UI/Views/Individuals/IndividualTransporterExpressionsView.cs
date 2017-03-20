using System.Linq;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Assets;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualTransporterExpressionsView : BaseUserControlWithValueInGrid, IIndividualTransporterExpressionsView
   {
      private readonly GridViewBinder<TransporterExpressionContainerDTO> _gridViewBinder;
      protected readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private readonly ScreenBinder<TransporterExpressionDTO> _screenBinder;
      private IIndividualTransporterExpressionsPresenter _presenter;
      protected readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};
      private IGridViewColumn _colGrouping;
      private readonly UxRepositoryItemImageComboBox _containerDisplayNameRepository;
      private IGridViewColumn _colRelativeExpression;

      public IndividualTransporterExpressionsView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         _screenBinder = new ScreenBinder<TransporterExpressionDTO>();
         gridView.AllowsFiltering = false;
         _gridViewBinder = new GridViewBinder<TransporterExpressionContainerDTO>(gridView) {BindingMode = BindingMode.OneWay};
         _containerDisplayNameRepository = new UxRepositoryItemImageComboBox(gridView, imageListRetriever);


         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.GroupFormat = "[#image]{1}";
         gridView.CustomColumnSort += customColumnSort;
         InitializeWithGrid(gridView);
         panelWarning.Image = ApplicationIcons.Warning;

         var toolTipController = new ToolTipController();
         toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         toolTipController.Initialize(_imageListRetriever);
         gridView.GridControl.ToolTipController = toolTipController;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var transporterContainerDTO = _gridViewBinder.ElementAt(e);
         if (transporterContainerDTO == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(transporterContainerDTO);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(transporterContainerDTO, superToolTip);
      }

      private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      {
         if (e.Column != _colGrouping.XtraColumn) return;
         var container1 = e.RowObject1 as ExpressionContainerDTO;
         var container2 = e.RowObject2 as ExpressionContainerDTO;
         if (container1 == null || container2 == null) return;
         e.Handled = true;

         e.Result = container1.Sequence.CompareTo(container2.Sequence);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.TransportType)
            .To(cbTransporterType)
            .WithImages(transporterIconIndex)
            .WithValues(x => _presenter.AllTransportTypes())
            .AndDisplays(x => _presenter.TransportTypeCaptionFor(x))
            .OnValueSet += (o, e) => OnEvent(() => _presenter.UpdateTransportType(e.NewValue));

         _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;

         _gridViewBinder.Bind(item => item.MembraneLocation)
            .WithRepository(getTransporterMembraneRepository)
            .WithEditorConfiguration(editTransporterMembraneTypeRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .OnValueSet += (transporter, args) => _presenter.SetMembraneLocation(transporter, args.NewValue);

         _colRelativeExpression = _gridViewBinder.Bind(item => item.RelativeExpression)
            .WithCaption(PKSimConstants.UI.RelativeExpression)
            .WithOnValueSet((protein, args) => _presenter.SetRelativeExpression(protein, args.NewValue));

         var col = _gridViewBinder.Bind(item => item.RelativeExpressionNorm)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(x => _progressBarRepository)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _colRelativeExpression.XtraColumn == column;
      }

      private string membraneContainerDisplayName(MembraneLocation membraneLocation, TransporterExpressionContainerDTO containerDTO)
      {
         return string.Format("{0} ({1})", containerDTO.ContainerPathDTO.DisplayName, membraneLocation);
      }

      private RepositoryItem configureContainerRepository(PathElementDTO parameterPathDTO)
      {
         _containerDisplayNameRepository.Items.Clear();
         _containerDisplayNameRepository.Items.Add(new ImageComboBoxItem(parameterPathDTO, _imageListRetriever.ImageIndex(parameterPathDTO.IconName)));
         return _containerDisplayNameRepository;
      }

      private RepositoryItem getTransporterMembraneRepository(TransporterExpressionContainerDTO containerDTO)
      {
         string displayName = containerDTO.ContainerPathDTO.DisplayName;
         string fullDisplayName = membraneContainerDisplayName(containerDTO.MembraneLocation, containerDTO);
         var allMembranesTypes = _presenter.AllProteinMembraneLocationsFor(containerDTO).ToList();
         if (allMembranesTypes.Count > 1)
            displayName = fullDisplayName;

         var repositoryItemImageComboBox = new UxRepositoryItemImageComboBox(gridView, _imageListRetriever) {ReadOnly = (allMembranesTypes.Count == 1), AllowDropDownWhenReadOnly = DefaultBoolean.False};
         if (repositoryItemImageComboBox.ReadOnly)
            repositoryItemImageComboBox.Buttons.Clear();

         var comboBoxItem = new ImageComboBoxItem(displayName, containerDTO.MembraneLocation, _imageListRetriever.ImageIndex(containerDTO.ContainerPathDTO.IconName));
         repositoryItemImageComboBox.Items.Add(comboBoxItem);
         return repositoryItemImageComboBox;
      }

      private void editTransporterMembraneTypeRepository(BaseEdit editor, TransporterExpressionContainerDTO containerDTO)
      {
         var allMembranesTypes = _presenter.AllProteinMembraneLocationsFor(containerDTO);
         if (allMembranesTypes.Count() == 1)
            return;

         editor.FillImageComboBoxEditorWith(_presenter.AllProteinMembraneLocationsFor(containerDTO), x => _imageListRetriever.ImageIndex(containerDTO.ContainerPathDTO.IconName), x => membraneContainerDisplayName(x, containerDTO));
      }

      private int transporterIconIndex(TransportType transportType)
      {
         return _imageListRetriever.ImageIndex(_presenter.IconFor(transportType));
      }

      public void AttachPresenter(IIndividualTransporterExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(TransporterExpressionDTO transporterExpressionDTO)
      {
         _screenBinder.BindToSource(transporterExpressionDTO);
         _gridViewBinder.BindToSource(transporterExpressionDTO.AllContainerExpressions);
         gridView.BestFitColumns();
      }

      public void ShowWarning(string warning)
      {
         layoutItemWarning.Visibility = LayoutVisibility.Always;
         panelWarning.NoteText = warning;
      }

      public void HideWarning()
      {
         layoutItemWarning.Visibility = LayoutVisibility.Never;
      }

      public void AddMoleculePropertiesView(IView view)
      {
         AddViewTo(layoutItemMoleculeProperties, view);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError || _gridViewBinder.HasError; }
      }

      public void Clear()
      {
         _gridViewBinder.DeleteBinding();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTransporterType.Text = PKSimConstants.UI.TransporterType.FormatForLabel();
         cbTransporterType.SetImages(_imageListRetriever);
         lblTransporterTypeDescription.AsDescription();
         lblTransporterTypeDescription.Text = PKSimConstants.UI.TransporterTypeDescription;
         layoutItemMoleculeProperties.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }
   }
}