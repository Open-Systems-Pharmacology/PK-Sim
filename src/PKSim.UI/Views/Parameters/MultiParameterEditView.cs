using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using PKSim.UI.Extensions;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Parameters
{
   public partial class MultiParameterEditView : ParameterSetView, IMultiParameterEditView
   {
      private readonly PathElementsBinder<ParameterDTO> _pathElementsBinder;
      private RepositoryItemTextEdit _repositoryForStandardParameter;
      private RepositoryItemProgressBar _progressBarRepository;

      private IGridViewColumn _columnPercentile;
      private IMultiParameterEditPresenter _presenter;
      private IList<PathElement> _groupingIndexList;
      private IGridViewColumn _columCategory;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public bool UseAdvancedSortingMode { set; private get; }
      public bool CustomSortEnabled { set; private get; }

      public MultiParameterEditView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, PathElementsBinder<ParameterDTO> pathElementsBinder)
         : base(toolTipCreator, imageListRetriever)
      {
         _pathElementsBinder = pathElementsBinder;
         InitializeComponent();
         Initialize(gridViewParameters);
         initRepositories();
         //Allow cell selection
         gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = true;
         gridViewParameters.EditorShowMode = EditorShowMode.MouseUp;
         gridViewParameters.CustomColumnSort += customColumnSort;
         gridViewParameters.EndGrouping += updateGrouping;
         CustomSortEnabled = true;
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
         _columCategory = _gridViewBinder.Bind(param => param.Category)
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
         _repositoryForStandardParameter = new RepositoryItemTextEdit();
         _progressBarRepository = new RepositoryItemProgressBar {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};
         _repositoryForStandardParameter.ReadOnly = true;
         _repositoryForStandardParameter.Enabled = false;
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
         gridViewParameters.BestFitColumns();
         AdjustHeight();
      }

      protected override void ShowPopup(IParameterDTO parameterDTO, Point location)
      {
         _presenter.CreatePopupMenuFor(parameterDTO).At(location);
      }

      public bool ParameterNameVisible
      {
         set
         {
            _pathElementsBinder.SetVisibility(PathElement.Name, visible: value);
            if (value)
               _pathElementsBinder.ColumnAt(PathElement.Name).XtraColumn.VisibleIndex = 0;
         }
         get { return _pathElementsBinder.ColumnAt(PathElement.Name).Visible; }
      }

      public bool DistributionVisible
      {
         get { return _columnPercentile.Visible; }
         set { _columnPercentile.UpdateVisibility(value); }
      }

      public bool CategoryVisible
      {
         get { return _columCategory.Visible; }
         set { _columCategory.UpdateVisibility(value); }
      }

      public void SetCaption(PathElement pathElement, string caption)
      {
         _pathElementsBinder.SetCaption(pathElement, caption);
      }

      public void SetVisibility(PathElement pathElement, bool visible)
      {
         _pathElementsBinder.SetVisibility(pathElement, visible);
      }

      public void GroupByCategory()
      {
         groupByColumn(_columCategory);
      }

      public void GroupBy(PathElement pathElement, int groupIndex = 0, bool useCustomSort = true)
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
         int rowHandle = gridViewParameters.GetVisibleRowHandle(0);
         gridViewParameters.FocusedRowHandle = rowHandle;
         gridViewParameters.ExpandAllGroups();
      }

      private IList<PathElement> indexOfGroupByColumns()
      {
         return _pathElementsBinder.ColumnCache.KeyValues
            .Where(columnKeyValue => isGroupBy(columnKeyValue.Value))
            .Select(columnKeyValue => columnKeyValue.Key).ToList();
      }

      private bool isGroupBy(IGridViewColumn gridViewColumn)
      {
         return gridViewColumn.XtraColumn.GroupIndex >= 0;
      }

      public void FixParameterColumnWidth(int parameterWitdh)
      {
         _columnValue.WithFixedWidth(parameterWitdh);
      }

      public IEnumerable<ParameterDTO> SelectedParameters
      {
         get { return gridViewParameters.DataController.GetAllFilteredAndSortedRows().Cast<ParameterDTO>(); }
      }

      public void SaveEditor()
      {
         gridViewParameters.PostEditor();
      }

      public bool ParameterPathVisible
      {
         set { _pathElementsBinder.PathVisible = value; }
      }

      public bool GroupingVisible
      {
         set { gridViewParameters.AllowsFiltering = value; }
         get { return gridViewParameters.AllowsFiltering; }
      }

      public bool HeaderVisible
      {
         set { gridViewParameters.ShowColumnHeaders = value; }
         get { return gridViewParameters.ShowColumnHeaders; }
      }

      public bool ShowRowIndicator
      {
         set { gridViewParameters.ShowRowIndicator = value; }
         get { return gridViewParameters.ShowRowIndicator; }
      }

      public bool ScalingVisible
      {
         set
         {
            layoutItemScaling.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            emptySpaceItemForScaling.Visibility = layoutItemScaling.Visibility;
         }
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemScaling.Visibility); }
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

      public int OptimalHeight
      {
         get { return gridViewParameters.OptimalHeight + Padding.All + 2; }
      }

      public bool AllowMultiSelect
      {
         set { gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = value; }
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
         Repaint();
      }

      public void Repaint()
      {
         gridViewParameters.LayoutChanged();
      }

      protected override bool ColumnIsAlwaysActive(GridColumn column)
      {
         if (column == null) return false;
         return column.IsOneOf(_columnFavorites.XtraColumn, _columnValueDescription.XtraColumn);
      }
   }
}