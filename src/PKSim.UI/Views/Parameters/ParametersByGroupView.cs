using System.Collections.Generic;
using System.Drawing;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Binders;

namespace PKSim.UI.Views.Parameters
{
   public partial class ParametersByGroupView : ParameterSetView, IParametersByGroupView
   {
      private IParametersByGroupPresenter _presenter;
      private IGridViewColumn _colGrouping;
      private IGridViewColumn _columnName;
      public bool Updating { get; private set; }

      public ParametersByGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, PKSim.UI.Binders.ValueOriginBinder<ParameterDTO> valueOriginBinder)
         : base(toolTipCreator, imageListRetriever, valueOriginBinder)
      {
         InitializeComponent();
         Initialize(gridViewParameters);
         gridViewParameters.AllowsFiltering = false;
         gridViewParameters.CustomColumnSort += customColumnSort;
      }

      private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      {
         if (e.Column != _colGrouping.XtraColumn) return;
         var parameterDTO1 = e.RowObject1 as ParameterDTO;
         var parameterDTO2 = e.RowObject2 as ParameterDTO;
         if (parameterDTO1 == null || parameterDTO2 == null) return;
         e.Handled = true;
         e.Result = parameterDTO1.Sequence.CompareTo(parameterDTO2.Sequence);
      }

      public void AttachPresenter(IParametersByGroupPresenter presenter)
      {
         base.AttachPresenter(presenter);
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _colGrouping = _gridViewBinder.AutoBind(x => x.SimulationPathElement).AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;
         InitializeNameBinding();
         InitializeValueBinding();
         InitializeValueDescriptionBinding();
         InitializeFavoriteBinding();
         gridViewParameters.CustomDrawGroupRow += removeCollapseExpandIndicator;
         gridViewParameters.GroupRowCollapsing += (o, e) => { e.Allow = false; };
      }

      private void removeCollapseExpandIndicator(object sender, RowObjectCustomDrawEventArgs e)
      {
         var info = e.Info as GridGroupRowInfo;
         if (info == null) return;
         info.ButtonBounds = Rectangle.Empty;
         info.GroupText = " " + info.GroupText.TrimStart();
         e.Info.Paint.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds);
         e.Painter.DrawObject(e.Info);
         e.Handled = true;
      }

      protected void InitializeNameBinding()
      {
         _columnName = _gridViewBinder.Bind(param => param.DisplayName)
            .WithCaption(PKSimConstants.UI.Name)
            .AsReadOnly();
      }

      protected override void ShowPopup(IParameterDTO parameterDTO, Point location)
      {
         _presenter.CreatePopupMenuFor(parameterDTO).At(location);
      }

      public int OptimalHeight => gridViewParameters.OptimalHeight;

      public void BindTo(IEnumerable<ParameterDTO> allParameters)
      {
         _gridViewBinder.BindToSource(allParameters.ToBindingList());
      }

      public void RefreshData()
      {
         gridParameters.RefreshDataSource();
      }

      public bool GroupingVisible
      {
         set
         {
            _colGrouping.XtraColumn.Visible = value;
            _colGrouping.XtraColumn.GroupIndex = value ? 0 : -1;
         }
      }

      public bool HeaderVisible
      {
         set { gridViewParameters.ShowColumnHeaders = value; }
      }

      public void BeginUpdate()
      {
         gridParameters.BeginUpdate();
         Updating = true;
      }

      public void EndUpdate()
      {
         gridParameters.EndUpdate();
         Updating = false;
      }
   }
}