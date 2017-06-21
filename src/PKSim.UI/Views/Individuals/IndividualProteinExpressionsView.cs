using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualProteinExpressionsView : BaseUserControlWithValueInGrid, IIndividualProteinExpressionsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};
      private readonly ScreenBinder<ProteinExpressionDTO> _screenBinder;
      protected IIndividualProteinExpressionsPresenter _presenter;
      private readonly UxRepositoryItemImageComboBox _containerDisplayNameRepository;
      private readonly GridViewBinder<ExpressionContainerDTO> _gridViewBinder;
      private IGridViewColumn _colGrouping;
      private IGridViewColumn _colRelativeExpression;

      public IndividualProteinExpressionsView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _screenBinder = new ScreenBinder<ProteinExpressionDTO>();
         gridView.AllowsFiltering = false;
         _containerDisplayNameRepository = new UxRepositoryItemImageComboBox(gridView, imageListRetriever);
         _gridViewBinder = new GridViewBinder<ExpressionContainerDTO>(gridView);
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.GroupFormat = "[#image]{1}";
         gridView.CustomColumnSort += customColumnSort;
         InitializeWithGrid(gridView);
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

      public void AttachPresenter(IIndividualProteinExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;

         _gridViewBinder.AutoBind(item => item.ContainerPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.ContainerPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();

         _colRelativeExpression = _gridViewBinder.Bind(item => item.RelativeExpression)
            .WithCaption(PKSimConstants.UI.RelativeExpression)
            .WithOnValueUpdating((protein, args) => _presenter.SetRelativeExpression(protein, args.NewValue));

         var col = _gridViewBinder.Bind(item => item.RelativeExpressionNorm)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(x => _progressBarRepository)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;

         _screenBinder.Bind(x => x.TissueLocation)
            .To(cbLocalizationInTissue)
            .WithImages(x => _imageListRetriever.ImageIndex(_presenter.IconFor(x)))
            .WithValues(x => _presenter.AllTissueLocations())
            .AndDisplays(x => _presenter.DisplayFor(x))
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.TissueLocationChanged(e.NewValue));

         _screenBinder.Bind(x => x.IntracellularVascularEndoLocation)
            .To(chkIntracellularVascularEndoLocation)
            .WithValues(x => _presenter.AllIntracellularVascularEndoLocations())
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.IntracellularLocationVascularEndoChanged(e.NewValue));

         _screenBinder.Bind(x => x.MembraneLocation)
            .To(cbLocationOnVascularEndo)
            .WithValues(x => _presenter.AllMembraneLocation())
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.MembraneLocationChanged(e.NewValue));

         RegisterValidationFor(_screenBinder,NotifyViewChanged);
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _colRelativeExpression.XtraColumn == column;
      }

      private RepositoryItem configureContainerRepository(PathElementDTO parameterPathDTO)
      {
         _containerDisplayNameRepository.Items.Clear();
         _containerDisplayNameRepository.Items.Add(new ImageComboBoxItem(parameterPathDTO, _imageListRetriever.ImageIndex(parameterPathDTO.IconName)));
         return _containerDisplayNameRepository;
      }

      public void BindTo(ProteinExpressionDTO proteinExpressionDTO)
      {
         _gridViewBinder.BindToSource(proteinExpressionDTO.AllContainerExpressions.ToBindingList());
         _screenBinder.BindToSource(proteinExpressionDTO);
         gridView.BestFitColumns();
      }

      public bool IntracellularVascularEndoLocationVisible
      {
         set { layoutItemIntracellularVascularEndoLocation.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public bool LocationOnVascularEndoVisible
      {
         set { layoutItemLocalizationInVascularEndo.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public void Clear()
      {
         _gridViewBinder.DeleteBinding();
      }

      public void AddMoleculePropertiesView(IView view)
      {
         AddViewTo(layoutItemMoleculeProperties, view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemLocalizationInTissue.Text = PKSimConstants.UI.LocalizationInTissue.FormatForLabel();
         layoutItemLocalizationInVascularEndo.Text = PKSimConstants.UI.LocationOnVascularEndo.FormatForLabel();
         layoutItemIntracellularVascularEndoLocation.Text = PKSimConstants.UI.IntracellularVascularEndoLocation.FormatForLabel();
         cbLocalizationInTissue.SetImages(_imageListRetriever);
         layoutItemMoleculeProperties.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError || _gridViewBinder.HasError; }
      }

      protected override int TopicId => HelpId.PKSim_Expression_Reference_Concentration;
   }
}