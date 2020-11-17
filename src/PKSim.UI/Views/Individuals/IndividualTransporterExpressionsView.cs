using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualTransporterExpressionsView : BaseContainerUserControl, IIndividualTransporterExpressionsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly ScreenBinder<IndividualTransporterDTO> _screenBinder;
      private IIndividualTransporterExpressionsPresenter _presenter;

      public IndividualTransporterExpressionsView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         _screenBinder = new ScreenBinder<IndividualTransporterDTO>();
         panelWarning.Image = ApplicationIcons.Warning;

      }

      // private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      // {
      //    var transporterContainerDTO = _gridViewBinder.ElementAt(e);
      //    if (transporterContainerDTO == null) return;
      //
      //    var superToolTip = _toolTipCreator.ToolTipFor(transporterContainerDTO);
      //    e.Info = _toolTipCreator.ToolTipControlInfoFor(transporterContainerDTO, superToolTip);
      // }


      public override void InitializeBinding()
      {
         // _screenBinder.Bind(x => x.TransportType)
         //    .To(cbTransporterType)
         //    .WithImages(transporterIconIndex)
         //    .WithValues(x => _presenter.AllTransportTypes())
         //    .AndDisplays(x => _presenter.TransportTypeCaptionFor(x))
         //    .OnValueUpdating += (o, e) => OnEvent(() => _presenter.UpdateTransportType(e.NewValue));
         //
         // _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
         //    .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
         //    .WithCaption(PKSimConstants.UI.EmptyColumn)
         //    .AsReadOnly();
         // _colGrouping.XtraColumn.GroupIndex = 0;
         // _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;

         //TODO 
         // _gridViewBinder.Bind(item => item.MembraneLocation)
         //    .WithRepository(getTransporterMembraneRepository)
         //    .WithEditorConfiguration(editTransporterMembraneTypeRepository)
         //    .WithShowButton(ShowButtonModeEnum.ShowAlways)
         //    .WithCaption(PKSimConstants.UI.EmptyColumn)
         //    .OnValueUpdating += (transporter, args) => _presenter.SetTransportDirection(transporter, args.NewValue);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

   

      private string membraneContainerDisplayName(MembraneLocation membraneLocation, TransporterExpressionParameterDTO containerDTO)
      {
         return $"{containerDTO.ContainerPathDTO.DisplayName} ({membraneLocation})";
      }
      //
      // private RepositoryItem configureContainerRepository(PathElement parameterPath)
      // {
      //    var containerDisplayNameRepository = new UxRepositoryItemImageComboBox(gridView, _imageListRetriever);
      //    return containerDisplayNameRepository.AddItem(parameterPath, parameterPath.IconName);
      // }

      // private RepositoryItem getTransporterMembraneRepository(TransporterExpressionContainerDTO containerDTO)
      // {
      //    string displayName = containerDTO.ContainerPathDTO.DisplayName;
      //    string fullDisplayName = membraneContainerDisplayName(containerDTO.MembraneLocation, containerDTO);
      //    var allMembranesTypes = _presenter.AllMembraneLocationsFor(containerDTO).ToList();
      //    if (allMembranesTypes.Count > 1)
      //       displayName = fullDisplayName;
      //
      //    var repositoryItemImageComboBox = new UxRepositoryItemImageComboBox(gridView, _imageListRetriever) {ReadOnly = (allMembranesTypes.Count == 1), AllowDropDownWhenReadOnly = DefaultBoolean.False};
      //    if (repositoryItemImageComboBox.ReadOnly)
      //       repositoryItemImageComboBox.Buttons.Clear();
      //
      //
      //    var comboBoxItem = new ImageComboBoxItem(displayName, containerDTO.MembraneLocation, _imageListRetriever.ImageIndex(containerDTO.ContainerPathDTO.IconName));
      //    repositoryItemImageComboBox.Items.Add(comboBoxItem);
      //    return repositoryItemImageComboBox;
      // }
      //
      // private void editTransporterMembraneTypeRepository(BaseEdit editor, TransporterExpressionParameterDTO containerDTO)
      // {
      //    var allMembranesTypes = _presenter.AllMembraneLocationsFor(containerDTO);
      //    if (allMembranesTypes.Count() == 1)
      //       return;
      //
      //    editor.FillImageComboBoxEditorWith(_presenter.AllMembraneLocationsFor(containerDTO), x => _imageListRetriever.ImageIndex(containerDTO.ContainerPathDTO.IconName), x => membraneContainerDisplayName(x, containerDTO));
      // }
      //
      // private int transporterIconIndex(TransportType transportType)
      // {
      //    return _imageListRetriever.ImageIndex(_presenter.IconFor(transportType));
      // }

      public void AttachPresenter(IIndividualTransporterExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IndividualTransporterDTO transporterExpressionDTO)
      {
         _screenBinder.BindToSource(transporterExpressionDTO);
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

      public void AddExpressionParametersView(IView view)
      {
         AddViewTo(layoutItemExpressionParameters, view);
      }

      public override bool HasError => _screenBinder.HasError;

 
      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTransporterType.Text = PKSimConstants.UI.TransporterType.FormatForLabel();
         cbTransporterType.SetImages(_imageListRetriever);
         lblTransporterTypeDescription.AsDescription();
         lblTransporterTypeDescription.Text = PKSimConstants.UI.TransporterTypeDescription;
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }
   }
}