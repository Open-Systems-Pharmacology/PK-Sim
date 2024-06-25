using System;
using System.Data;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using PKSim.Assets;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.ProteinExpression
{
   internal partial class ProteinSelectionView : BaseUserControl, IProteinSelectionView
   {
      private readonly IProteinExpressionToolTipCreator _toolTipCreator;
      private IProteinSelectionPresenter _presenter;

      public bool SelectionChanged { get; private set; } = false;

      public string SearchCriteria
      {
         set => txtSearchCriteria.Text = value;
      }

      public void Activate()
      {
         ActiveControl = txtSearchCriteria;
         txtSearchCriteria.Select();
         txtSearchCriteria.SuperTip = _toolTipCreator.GetTipForSearchCriteriaText();
      }

      public ProteinSelectionView(IProteinExpressionToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();

         btnSearch.Enabled = false;

         btnSearch.Click += (s, e) => doProteinSearch();
         txtSearchCriteria.Leave += (s, e) =>
         {
            if (btnSearch.Enabled) doProteinSearch();
         };
         txtSearchCriteria.Properties.ValidateOnEnterKey = true;
         txtSearchCriteria.Validating += (s, e) =>
         {
            if (btnSearch.Enabled) doProteinSearch();
         };
         txtSearchCriteria.EditValueChanged += onSearchCriteriaChanged;

         grdSelection.DoubleClick += (s, e) => doProteinSelection();
         grdSelection.ToolTipController = new ToolTipController();
         grdSelection.ToolTipController.Initialize();
         grdSelection.ToolTipController.GetActiveObjectInfo += onGetActiveObjectInfo;
         var view = grdSelection.FocusedView as ColumnView;
         if (view != null) view.FocusedRowChanged += onFocusedRowChanged;

         var gridView = grdSelection.FocusedView as GridView;
         if (gridView != null) gridView.RowStyle += OnGridViewRowStyle;
      }

      /// <summary>
      ///    This event handler highlights all rows which have no expression data.
      /// </summary>
      private static void OnGridViewRowStyle(object sender, RowStyleEventArgs e)
      {
         var gridView = sender as GridView;
         var row = gridView?.GetDataRow(e.RowHandle);
         if (row == null)
            return;

         var hasData = row.ValueAt<long>(DatabaseConfiguration.ProteinColumns.HAS_DATA);
         if (hasData != 1)
            e.Appearance.BackColor = Color.LightGray;
      }

      /// <summary>
      ///    This event handler recognize each input in the search criteria field and enables the search button.
      /// </summary>
      void onSearchCriteriaChanged(object sender, EventArgs e)
      {
         var textbox = sender as TextEdit;
         if (textbox?.EditValue != null)
            btnSearch.Enabled = (textbox.EditValue.ToString().Length > 0);
      }

      void onFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
      {
         SelectionChanged = true;
         var view = sender as GridView;
         var row = view?.GetDataRow(e.FocusedRowHandle);
         if (row == null)
            return;

         var hasData = row.ValueAt<long>(DatabaseConfiguration.ProteinColumns.HAS_DATA);
         _presenter.ProteinHasData = (hasData == 1);
         _presenter.SetActiveControl();
      }

      /// <summary>
      ///    This function handle the event which takes care of tool tips.
      ///    The code checks on which area the mouse currently is positioned.
      ///    Depending on the area and the field and the field value different tool tips are generated.
      /// </summary>
      /// <remarks>The object reference for a new ToolTipControlInfo object can be any unique string.</remarks>
      private void onGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != grdSelection) return;
         if (grdSelection.DataSource == null) return;

         ToolTipControlInfo info = null;

         //Get the view at the current mouse position
         var view = grdSelection.GetViewAt(e.ControlMousePosition) as GridView;
         if (view == null) return;

         //Get the view's element information that resides at the current position
         GridHitInfo hi = view.CalcHitInfo(e.ControlMousePosition);

         //Display a hint for row cells
         if (hi.HitTest == GridHitTest.RowCell || hi.HitTest == GridHitTest.Column)
         {
            //An object that uniquely identifies a row cell
            object o = $"Row {hi.RowHandle}-{hi.Column.FieldName}";
            info = new ToolTipControlInfo(o, string.Empty);
            if (hi.Column.FieldName == DatabaseConfiguration.ProteinColumns.COL_GENE_ID ||
                hi.Column.FieldName == DatabaseConfiguration.ProteinColumns.COL_SYMBOL ||
                hi.Column.FieldName == DatabaseConfiguration.ProteinColumns.COL_OFFICIAL_FULL_NAME)
            {
               string nameType = hi.Column.FieldName;
               info.SuperTip = _toolTipCreator.GetTipForThisNameType(hi.Column.Caption, nameType);
               info.ToolTipType = ToolTipType.SuperTip;
            }
            else if (hi.Column.FieldName == DatabaseConfiguration.ProteinColumns.COL_NAME_TYPE)
            {
               info.SuperTip = _toolTipCreator.GetTipForNameType(hi.Column.Caption);
               info.ToolTipType = ToolTipType.SuperTip;
            }
            else if (hi.Column.FieldName == DatabaseConfiguration.ProteinColumns.COL_GENE_NAME)
            {
               info.SuperTip = _toolTipCreator.GetTipForGeneName(hi.Column.Caption);
               info.ToolTipType = ToolTipType.SuperTip;
            }
         }

         //Supply tooltip information if applicable, otherwise preserve default tooltip (if any)
         if (info != null)
            e.Info = info;
      }

      public void SetAvailableProteinsForSearchCriteria(DataTable selectionTable)
      {
         grdSelection.DataSource = selectionTable;
         configGrid();
         var view = grdSelection.FocusedView as GridView;
         if (view == null) return;
         view.FocusedRowHandle = -1;

         if (selectionTable.Rows.Count > 0)
            view.FocusedRowHandle = 0;
      }

      public void ActualizeSelection()
      {
         doProteinSelection();
      }

      public DataRow SelectedProteinData
      {
         get
         {
            // find selected id and get expression data
            var view = grdSelection.FocusedView as ColumnView;
            SelectionChanged = false;
            return view == null ? null : view.GetDataRow(view.FocusedRowHandle);
         }
      }

      private void configGrid()
      {
         grdSelection.Visible = true;
         var view = grdSelection.MainView as GridView;
         if (view == null) return;

         view.OptionsView.ShowGroupPanel = false;
         view.OptionsBehavior.Editable = true; // must be set to true to support hyper link for gene id.

         view.Columns[DatabaseConfiguration.ProteinColumns.COL_ID].Visible = false;
         foreach (GridColumn col in view.Columns)
         {
            switch (col.FieldName)
            {
               case DatabaseConfiguration.ProteinColumns.COL_GENE_ID:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_GENE_ID;
                  break;
               case DatabaseConfiguration.ProteinColumns.COL_GENE_NAME:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_GENE_NAME;
                  break;
               case DatabaseConfiguration.ProteinColumns.COL_ID:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_ID;
                  break;
               case DatabaseConfiguration.ProteinColumns.COL_NAME_TYPE:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_NAME_TYPE;
                  break;
               case DatabaseConfiguration.ProteinColumns.COL_OFFICIAL_FULL_NAME:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_OFFICIAL_FULL_NAME;
                  break;
               case DatabaseConfiguration.ProteinColumns.COL_SYMBOL:
                  col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ProteinSelection.COL_SYMBOL;
                  break;
            }

            // must be set to true for Gene ID column to support hyper link.
            col.OptionsColumn.AllowEdit = (col.FieldName == DatabaseConfiguration.ProteinColumns.COL_GENE_ID);
         }
         view.Columns[DatabaseConfiguration.ProteinColumns.HAS_DATA].Visible = false;

         var hyperLinkEditor = new RepositoryItemHyperLinkEdit();
         hyperLinkEditor.ReadOnly = true;
         hyperLinkEditor.SingleClick = true; //means you need just a single click to open url.
         hyperLinkEditor.OpenLink += onOpenLink;
         view.Columns[DatabaseConfiguration.ProteinColumns.COL_GENE_ID].ColumnEdit = hyperLinkEditor;
      }

      private void onOpenLink(object sender, OpenLinkEventArgs e)
      {
         const string STR_GENE_NAME_DB_BASE_URL = "http://www.ncbi.nlm.nih.gov/gene/";
         e.EditValue = string.Concat(STR_GENE_NAME_DB_BASE_URL, e.EditValue);
      }

      private void doProteinSearch()
      {
         string criteria = txtSearchCriteria.Text;
         //if criteria is not quoted, surround it with wildcards
         if ((criteria.StartsWith("'") && criteria.EndsWith("'")) ||
             (criteria.StartsWith("\"") && criteria.EndsWith("\"")))
            criteria = criteria.Substring(1, criteria.Length - 2);
         else
            criteria = String.Concat("*", criteria, "*");

         criteria = criteria.Replace("*", "%");
         criteria = criteria.Replace("?", "_");
         this.DoWithinWaitCursor(() => _presenter.SearchProtein(criteria));
         SelectionChanged = true;
         btnSearch.Enabled = false;
      }

      private void doProteinSelection()
      {
         _presenter.SelectProtein();
      }

      public void AttachPresenter(IProteinSelectionPresenter presenter)
      {
         _presenter = presenter;
         grdSelection.Visible = false;
      }

      public override void InitializeResources()
      {
         layoutItemSearchCriteria.Text = PKSimConstants.ProteinExpressions.PageSelection.LabelSearchCriteria.FormatForLabel();
         btnSearch.Text = PKSimConstants.ProteinExpressions.PageSelection.ButtonSearch;
         Caption = PKSimConstants.ProteinExpressions.MainView.TabPageSelection;
         ApplicationIcon = ApplicationIcons.ProteinExpression;
         layoutItemSearch.AdjustButtonSize(layoutControl);
      }
   }
}