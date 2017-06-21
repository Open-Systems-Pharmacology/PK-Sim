using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using PKSim.UI.Views.Parameters;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationExpressionsView : BaseUserControlWithValueInGrid, ISimulationExpressionsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private ISimulationExpressionsPresenter _presenter;
      private readonly GridViewBinder<ExpressionContainerDTO> _gridViewBinder;
      private readonly ScreenBinder<SimulationExpressionsDTO> _screenBinder;
      private readonly UxParameterDTOEdit _uxReferenceConcentration;
      private readonly UxParameterDTOEdit _uxHalfLifeLiver;
      private readonly UxParameterDTOEdit _uxHalfLifeIntestine;
      private readonly UxRepositoryItemImageComboBox _containerDisplayNameRepository;
      private IGridViewColumn _colGrouping;
      private readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar { Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true };
      private readonly RepositoryItem _favoriteRepository;
      private IGridViewColumn _columnValue;
      private readonly ToolTipController _toolTipController;

      public SimulationExpressionsView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         InitializeWithGrid(gridViewParameters);
         _toolTipController = new ToolTipController();

         _gridViewBinder = new GridViewBinder<ExpressionContainerDTO>(gridViewParameters)
            {
               BindingMode = BindingMode.OneWay
            };
          _screenBinder = new ScreenBinder<SimulationExpressionsDTO>();
         _favoriteRepository = new UxRepositoryItemCheckEdit(gridViewParameters);
         _uxReferenceConcentration = new UxParameterDTOEdit();
         _uxHalfLifeLiver = new UxParameterDTOEdit();
         _uxHalfLifeIntestine = new UxParameterDTOEdit();
         _containerDisplayNameRepository = new UxRepositoryItemImageComboBox(gridViewParameters, _imageListRetriever);
         gridViewParameters.GroupFormat = "[#image]{1}";
         gridViewParameters.EndGrouping += (o, e) => gridViewParameters.ExpandAllGroups();
         gridViewParameters.AllowsFiltering = false;
         gridViewParameters.CustomColumnSort += customColumnSort;
         gridViewParameters.GridControl.ToolTipController = _toolTipController;
         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e);
         if (parameterDTO == null) return;

         var superToolTip = _toolTipCreator.CreateToolTip(parameterDTO.ParameterPath);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDTO, superToolTip);
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

         _columnValue = _gridViewBinder.Bind(item => item.RelativeExpression)
            .WithCaption(PKSimConstants.UI.RelativeExpression)
            .WithOnValueUpdating((protein, args) => _presenter.SetRelativeExpression(protein, args.NewValue));

         var col = _gridViewBinder.Bind(item => item.RelativeExpressionNorm)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(x => _progressBarRepository)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;


         _gridViewBinder.Bind(x => x.IsFavorite)
            .WithCaption(PKSimConstants.UI.Favorites)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH)
            .WithRepository(x => _favoriteRepository)
            .WithToolTip(PKSimConstants.UI.FavoritesToolTip)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetFavorite(o, e.NewValue)));


         _screenBinder.Bind(x => x.ReferenceConcentration).To(_uxReferenceConcentration);
         _uxReferenceConcentration.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.HalfLifeLiver).To(_uxHalfLifeLiver);
         _uxHalfLifeLiver.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.HalfLifeIntestine).To(_uxHalfLifeIntestine);
         _uxHalfLifeIntestine.RegisterEditParameterEvents(_presenter);
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_columnValue == null) return false;
         return _columnValue.XtraColumn == gridColumn;
      }

      private RepositoryItem configureContainerRepository(PathElementDTO parameterPathDTO)
      {
         _containerDisplayNameRepository.Items.Clear();
         _containerDisplayNameRepository.Items.Add(new ImageComboBoxItem(parameterPathDTO, _imageListRetriever.ImageIndex(parameterPathDTO.IconName)));
         return _containerDisplayNameRepository;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         panelReferenceConcentration.FillWith(_uxReferenceConcentration.DowncastTo<Control>());
         layoutItemReferenceConcentration.Text = PKSimConstants.UI.ReferenceConcentration.FormatForLabel();

         panelHalfLifeLiver.FillWith(_uxHalfLifeLiver.DowncastTo<Control>());
         layoutItemHalfLifeLiver.Text = PKSimConstants.UI.HalfLifeLiver.FormatForLabel();

         panelHalfLifeIntestine.FillWith(_uxHalfLifeIntestine.DowncastTo<Control>());
         layoutItemHalfLifeIntestine.Text = PKSimConstants.UI.HalfLifeIntestine.FormatForLabel();

      }

      public void AttachPresenter(ISimulationExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SimulationExpressionsDTO simulationExpressionsDTO)
      {
         _screenBinder.BindToSource(simulationExpressionsDTO);
         _gridViewBinder.BindToSource(simulationExpressionsDTO.RelativeExpressions.ToBindingList());
         _colGrouping.XtraColumn.GroupIndex = 0;
      }
   }
}