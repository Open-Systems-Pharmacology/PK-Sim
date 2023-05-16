using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.UI.Views.Parameters
{
   public partial class MultiParameterEditView : ParameterSetView, IMultiParameterEditView
   {
      private readonly PathElementsBinder<ParameterDTO> _pathElementsBinder;
      private RepositoryItemTextEdit _repositoryForStandardParameter;
      private RepositoryItemProgressBar _progressBarRepository;

      private IGridViewColumn _columnPercentile;
      private IMultiParameterEditPresenter _presenter;
      private IList<PathElementId> _groupingIndexList;
      private IGridViewColumn _columnCategory;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public bool UseAdvancedSortingMode { set; private get; }
      public bool CustomSortEnabled { set; private get; }

      public MultiParameterEditView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, ValueOriginBinder<ParameterDTO> valueOriginBinder, PathElementsBinder<ParameterDTO> pathElementsBinder)
         : base(toolTipCreator, imageListRetriever, valueOriginBinder)
      {
         _pathElementsBinder = pathElementsBinder;
         InitializeComponent();
         Initialize(gridView);
         initRepositories();
         //Allow cell selection
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
         gridView.EditorShowMode = EditorShowMode.MouseUp;
         gridView.CustomColumnSort += customColumnSort;
         gridView.EndGrouping += updateGrouping;
         CustomSortEnabled = true;
         gridView.SelectionChanged += (o, e) => OnEvent(_presenter.SelectedParametersChanged);
      }

      private void updateGrouping(object sender, EventArgs e)
      {
         _groupingIndexList = indexOfGroupByColumns();
      }

      public override void InitializeBinding()
      {
         InitializePathBinding();
         InitializeValueBinding();
         InitializeCategoryBinding();
         InitializePercentileBinding();
         InitializeValueDescriptionBinding();
         InitializeFavoriteBinding();

         //reset format for use with progress bar
         _columnPercentile.XtraColumn.DisplayFormat.FormatType = FormatType.None;
         //necessary to align center since double value are aligned right by default
         _columnPercentile.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
      }

      protected void InitializePathBinding()
      {
         _pathElementsBinder.InitializeBinding(_gridViewBinder);
      }

      protected void InitializePercentileBinding()
      {
         _columnPercentile = _gridViewBinder.Bind(param => param.Percentile)
            .WithCaption(PKSimConstants.UI.Percentile)
            .WithRepository(getPercentileRepository)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetParameterPercentile(o, e.NewValue)))
            .AsReadOnly();
      }

      protected void InitializeCategoryBinding()
      {
         _columnCategory = _gridViewBinder.Bind(param => param.Category)
            .WithCaption(PKSimConstants.UI.Category)
            .AsReadOnly();
      }

      protected override SuperToolTip GetToolTipFor(IParameterDTO parameterDTO, GridHitInfo hi)
      {
         if (hi.Column == _columnFavorites.XtraColumn)
            return _toolTipCreator.CreateToolTip(PKSimConstants.UI.AddParameterAsFavorites(parameterDTO.DisplayName), PKSimConstants.UI.FavoritesToolTip);

         return base.GetToolTipFor(parameterDTO, hi);
      }

      private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      {
         if (!CustomSortEnabled) return;
         var parameterDTO1 = e.RowObject1 as ParameterDTO;
         var parameterDTO2 = e.RowObject2 as ParameterDTO;
         if (parameterDTO1 == null || parameterDTO2 == null) return;

         if (UseAdvancedSortingMode &&
             _groupingIndexList.Any(pathColumn => parameterDTO1.PathElements[pathColumn] != parameterDTO2.PathElements[pathColumn]))
            return;

         e.Handled = true;
         e.Result = parameterDTO1.Sequence.CompareTo(parameterDTO2.Sequence);
      }

      private RepositoryItem getPercentileRepository(IParameterDTO parameterDTO)
      {
         if (_presenter.ParameterIsDistributed(parameterDTO))
            return _progressBarRepository;
         return _repositoryForStandardParameter;
      }

      private void initRepositories()
      {
         _repositoryForStandardParameter = new RepositoryItemTextEdit {ReadOnly = true, Enabled = false};
         _progressBarRepository = new RepositoryItemProgressBar {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};
         _repositoryForStandardParameter.CustomDisplayText += (o, e) => { e.DisplayText = string.Empty; };
      }

      public void AttachPresenter(IMultiParameterEditPresenter presenter)
      {
         base.AttachPresenter(presenter);
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<ParameterDTO> parameters)
      {
         //necessary to bind to binding list to enable automatic update from value since we are 
         //using the autobind property from devexpress which only (!!) reacts to IBindingList properties.
         _gridViewBinder.BindToSource(parameters.ToBindingList());
         AdjustHeight();
      }

      protected override void ShowPopup(IParameterDTO parameterDTO, Point location)
      {
         _presenter.CreatePopupMenuFor(parameterDTO).At(location);
      }

      public bool ValueOriginVisible
      {
         get => _valueOriginBinder.ValueOriginVisible;
         set => _valueOriginBinder.ValueOriginVisible = value;
      }

      public bool ParameterNameVisible
      {
         set
         {
            _pathElementsBinder.SetVisibility(PathElementId.Name, visible: value);
            if (value)
               _pathElementsBinder.ColumnAt(PathElementId.Name).XtraColumn.VisibleIndex = 0;
         }
         get => _pathElementsBinder.ColumnAt(PathElementId.Name).Visible;
      }

      public bool DistributionVisible
      {
         get => _columnPercentile.Visible;
         set => _columnPercentile.UpdateVisibility(value);
      }

      public bool CategoryVisible
      {
         get => _columnCategory.Visible;
         set => _columnCategory.UpdateVisibility(value);
      }

      public void SetCaption(PathElementId pathElement, string caption)
      {
         _pathElementsBinder.SetCaption(pathElement, caption);
      }

      public void SetVisibility(PathElementId pathElement, bool visible)
      {
         _pathElementsBinder.SetVisibility(pathElement, visible);
      }

      public void GroupByCategory()
      {
         groupByColumn(_columnCategory);
      }

      public void GroupBy(PathElementId pathElement, int groupIndex = 0, bool useCustomSort = true)
      {
         groupByColumn(_pathElementsBinder.ColumnAt(pathElement), groupIndex, useCustomSort);
      }

      private void groupByColumn(IGridViewColumn gridViewColumn, int groupIndex = 0, bool useCustomSort = true)
      {
         if (!gridViewColumn.Visible) return;

         gridViewColumn.XtraColumn.GroupIndex = groupIndex;

         if (useCustomSort)
            gridViewColumn.XtraColumn.SortMode = ColumnSortMode.Custom;

         //set the first row visible
         int rowHandle = gridView.GetVisibleRowHandle(0);
         gridView.FocusedRowHandle = rowHandle;
         gridView.ExpandAllGroups();
      }

      private IList<PathElementId> indexOfGroupByColumns()
      {
         return _pathElementsBinder.ColumnCache.KeyValues
            .Where(columnKeyValue => isGroupBy(columnKeyValue.Value))
            .Select(columnKeyValue => columnKeyValue.Key).ToList();
      }

      private bool isGroupBy(IGridViewColumn gridViewColumn)
      {
         return gridViewColumn.XtraColumn.GroupIndex >= 0;
      }

      public void FixParameterColumnWidth(int parameterWidth)
      {
         _columnValue.WithFixedWidth(parameterWidth);
      }

      public IEnumerable<ParameterDTO> AllVisibleParameters => gridView.DataController.GetAllFilteredAndSortedRows().Cast<ParameterDTO>();

      public IReadOnlyList<ParameterDTO> SelectedParameters
      {
         get { return gridView.GetSelectedRows().Select(rowHandle => _gridViewBinder.ElementAt(rowHandle)).ToList(); }
         set
         {
            if (!value.Any())
               return;

            //Need to clear selection before setting another one programatically. Otherwise they overlap
            gridView.ClearSelection();

            var firstRowHandle = _gridViewBinder.RowHandleFor(value.First());
            var lastRowHandle = _gridViewBinder.RowHandleFor(value.Last());
            gridView.SelectRows(firstRowHandle, lastRowHandle);

            //Required to ensure that the background is still selected
            if (firstRowHandle == lastRowHandle)
               gridView.FocusedRowHandle = firstRowHandle;
         }
      }

      public bool AllowVerticalScrolling
      {
         set => gridView.VertScrollVisibility = value ? ScrollVisibility.Auto : ScrollVisibility.Never;
      }

      public void SaveEditor()
      {
         gridView.PostEditor();
      }

      public bool ParameterPathVisible
      {
         set => _pathElementsBinder.PathVisible = value;
      }

      public bool GroupingVisible
      {
         set => gridView.AllowsFiltering = value;
         get => gridView.AllowsFiltering;
      }

      public bool HeaderVisible
      {
         set => gridView.ShowColumnHeaders = value;
         get => gridView.ShowColumnHeaders;
      }

      public bool ShowRowIndicator
      {
         set => gridView.ShowRowIndicator = value;
         get => gridView.ShowRowIndicator;
      }

      public bool ScalingVisible
      {
         set
         {
            layoutItemScaling.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            emptySpaceItemForScaling.Visibility = layoutItemScaling.Visibility;
         }
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemScaling.Visibility);
      }

      public void SetScaleParameterView(IScaleParametersView view)
      {
         panelScaling.FillWith(view);
      }

      public void DeleteBinding()
      {
         _gridViewBinder.DeleteBinding();
      }

      public void RefreshData()
      {
         gridParameters.RefreshDataSource();
      }

      public int OptimalHeight => gridView.OptimalHeight ;

      public bool AllowMultiSelect
      {
         set => gridView.OptionsSelection.EnableAppearanceFocusedRow = value;
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
         Repaint();
      }

      public void Repaint()
      {
         gridView.LayoutChanged();
      }

      protected override bool ColumnIsAlwaysActive(GridColumn column)
      {
         if (column == null) return false;
         return column.IsOneOf(_columnFavorites.XtraColumn) || _valueOriginBinder.ColumnIsValueOrigin(column);
      }
   }
}