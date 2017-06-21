using System.Collections.Generic;
using System.ComponentModel;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Parameters
{
   public partial class TableParameterView : BaseUserControlWithValueInGrid, ITableParameterView
   {
      private ITableParameterPresenter _presenter;
      private readonly GridViewBinder<ValuePointDTO> _gridViewBinder;
      protected IGridViewColumn _columnX;
      protected IGridViewColumn _columnY;
      private readonly UxRepositoryItemButtonEdit _removePointRepository = new UxRemoveButtonRepository();
      private IGridViewColumn _removeColumn;
      private bool _editable;

      public TableParameterView()
      {
         InitializeComponent();
         InitializeWithGrid(gridView);
         gridView.AllowsFiltering = false;
         _editable = true;
         _gridViewBinder = new GridViewBinder<ValuePointDTO>(gridView);
         gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
      }

      public override void InitializeBinding()
      {
         gridView.ShowingEditor += onShowingEditor;

         _columnX = _gridViewBinder.AutoBind(x => x.X)
            .WithOnValueUpdating((o, e) => _presenter.SetXValue(o, e.NewValue));

         _columnY = _gridViewBinder.AutoBind(x => x.Y)
            .WithOnValueUpdating((o, e) => _presenter.SetYValue(o, e.NewValue));

         _removeColumn = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => _removePointRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH);

         _gridViewBinder.Changed += () => OnEvent(_presenter.ViewChanged);
         btnImportPoints.Click += (o, e) => OnEvent(_presenter.ImportTable);
         btnAddPoint.Click += (o, e) => OnEvent(_presenter.AddPoint);
         _removePointRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.RemovePoint(_gridViewBinder.FocusedElement));
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         e.Cancel = !_editable;
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _columnX.XtraColumn == column || _columnX.XtraColumn == column;
      }

      public void AttachPresenter(ITableParameterPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblImportDescription.AsDescription();
         btnImportPoints.Image = ApplicationIcons.LoadFromTemplate.ToImage(IconSizes.Size16x16);
         btnImportPoints.ImageLocation = ImageLocation.MiddleCenter;
         btnAddPoint.Image = ApplicationIcons.Create.ToImage(IconSizes.Size16x16);
         btnAddPoint.ImageLocation = ImageLocation.MiddleCenter;
         layoutItemImportPoints.AdjustButtonSizeWithImageOnly();
         layoutItemButtonAddPoint.AdjustButtonSizeWithImageOnly();
         lblImportDescription.Text = string.Empty;
         btnAddPoint.ToolTip = PKSimConstants.UI.AddPoint;
      }

      public void Clear()
      {
         _gridViewBinder.DeleteBinding();
      }

      public void BindTo(IEnumerable<ValuePointDTO> allPoints)
      {
         _gridViewBinder.BindToSource(allPoints);
      }

      public void EditPoint(ValuePointDTO pointToEdit)
      {
         var rowHandle = _gridViewBinder.RowHandleFor(pointToEdit);
         gridView.FocusedRowHandle = rowHandle;
         gridView.ShowEditor();
      }

      public bool ImportVisible
      {
         set { layoutItemImportPoints.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public string YCaption
      {
         set { _columnY.Caption = value; }
      }

      public string XCaption
      {
         set { _columnX.Caption = value; }
      }

      public bool Editable
      {
         set
         {
            _editable = value;
            layoutItemImportPoints.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            layoutItemButtonAddPoint.Visibility = layoutItemImportPoints.Visibility;
            gridView.MultiSelect = !value;
            _removeColumn.UpdateVisibility(value);
         }
         get { return _editable; }
      }

      public string Description
      {
         set { lblImportDescription.Text = value.FormatForDescription(); }
      }

      public string ImportToolTip
      {
         set { btnImportPoints.ToolTip = value; }
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError; }
      }
   }
}