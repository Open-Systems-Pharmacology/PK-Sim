using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Core.Domain;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using PKSim.UI.Views.Parameters;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationExpressionsView : BaseUserControlWithValueInGrid, ISimulationExpressionsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private ISimulationExpressionsPresenter _presenter;
      private readonly GridViewBinder<ExpressionParameterDTO> _gridViewBinder;
      private readonly ScreenBinder<SimulationExpressionsDTO> _screenBinder;
      private readonly UxParameterDTOEdit _uxReferenceConcentration;
      private readonly UxParameterDTOEdit _uxHalfLifeLiver;
      private readonly UxParameterDTOEdit _uxHalfLifeIntestine;
      private IGridViewColumn _colGrouping;
      private readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};
      private readonly RepositoryItem _favoriteRepository;
      private IGridViewColumn _columnValue;

      public SimulationExpressionsView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         InitializeWithGrid(gridViewParameters);
         var toolTipController = new ToolTipController();

         _gridViewBinder = new GridViewBinder<ExpressionParameterDTO>(gridViewParameters)
         {
            BindingMode = BindingMode.OneWay
         };
         _screenBinder = new ScreenBinder<SimulationExpressionsDTO>();
         _favoriteRepository = new UxRepositoryItemCheckEdit(gridViewParameters);
         _uxReferenceConcentration = new UxParameterDTOEdit();
         _uxHalfLifeLiver = new UxParameterDTOEdit();
         _uxHalfLifeIntestine = new UxParameterDTOEdit();
         gridViewParameters.GroupFormat = "[#image]{1}";
         gridViewParameters.EndGrouping += (o, e) => gridViewParameters.ExpandAllGroups();
         gridViewParameters.AllowsFiltering = false;
         // gridViewParameters.CustomColumnSort += customColumnSort;
         gridViewParameters.GridControl.ToolTipController = toolTipController;
         toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e);
         if (parameterDTO == null) return;

         var superToolTip = _toolTipCreator.CreateToolTip(parameterDTO.ParameterPath);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDTO, superToolTip);
      }
      //
      // private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      // {
      //    if (e.Column != _colGrouping.XtraColumn) return;
      //    var container1 = e.RowObject1 as ExpressionContainerDTO;
      //    var container2 = e.RowObject2 as ExpressionContainerDTO;
      //    if (container1 == null || container2 == null) return;
      //    e.Handled = true;
      //
      //    e.Result = container1.Sequence.CompareTo(container2.Sequence);
      // }

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

         _columnValue = _gridViewBinder.Bind(item => item.Value)
            .WithCaption(PKSimConstants.UI.RelativeExpression)
            .WithOnValueUpdating((o, args) => _presenter.SetRelativeExpression(o, args.NewValue));

         var col = _gridViewBinder.Bind(item => item.NormalizedExpressionPercent)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(x => _progressBarRepository)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;


         _gridViewBinder.Bind(x => x.IsFavorite)
            .WithCaption(PKSimConstants.UI.Favorites)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH)
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

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutGroupMoleculeParameters.Text = PKSimConstants.UI.Properties;
      }

      private RepositoryItem configureContainerRepository(PathElement parameterPath)
      {
         var containerDisplayNameRepository = new UxRepositoryItemImageComboBox(gridViewParameters, _imageListRetriever);
         return containerDisplayNameRepository.AddItem(parameterPath, parameterPath.IconName);
      }

      public void AttachPresenter(ISimulationExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SimulationExpressionsDTO simulationExpressionsDTO)
      {
         _screenBinder.BindToSource(simulationExpressionsDTO);
         _gridViewBinder.BindToSource(simulationExpressionsDTO.ExpressionParameters.ToBindingList());
         _colGrouping.XtraColumn.GroupIndex = 0;
      }

      public void AddMoleculeParametersView(IView view)
      {
         AddViewTo(layoutItemMoleculeParameters, view);
      }
   }
}