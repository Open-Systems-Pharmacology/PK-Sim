using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OSPSuite.Assets;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using PKSim.Assets;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.ProteinExpression
{
   internal partial class MappingView : BaseModalView, IMappingView
   {
      private IMappingPresenter _presenter;

      public MappingView(Shell shell): base(shell)
      {
         InitializeComponent();
         InitializeResources();

         btnCancel.Click += onCancelClick;
         btnOk.Click += onOkClick;

         layoutControl1.AllowCustomization = false;
         var view = grdMappping.MainView as GridView;
         if (view == null) return;
         view.CustomDrawCell += OnCustomDrawCell;
         view.CustomColumnDisplayText += OnCustomColumnDisplayText;
         view.RowCellStyle += onRowCellStyle;
         view.RowCellClick += OnRowCellClick;
         grdMappping.UseEmbeddedNavigator = true;
         view.OptionsBehavior.AllowAddRows = DefaultBoolean.True;
         view.OptionsBehavior.AllowDeleteRows = DefaultBoolean.True;
         view.OptionsCustomization.AllowColumnMoving = false;
         view.OptionsView.ShowGroupPanel = false;
         view.OptionsMenu.EnableGroupPanelMenu = false;
      }

      public void AttachPresenter(IMappingPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Text = PKSimConstants.ProteinExpressions.MappingView.Caption;
      }

      private void onOkClick(object sender, EventArgs e)
      {
         _presenter.SaveMapping(grdMappping.DataSource as DataTable);
      }

      private void onCancelClick(object sender, EventArgs e)
      {
         _presenter.CancelChanged(grdMappping.DataSource as DataTable);
      }

      public void SetData(DataTable mapping, DataTable containerTable, ICollection tissueLov)
      {
         grdMappping.DataSource = mapping;
         var view = grdMappping.MainView as GridView;
         if (view == null) return;
         GridColumn col;

         col = view.Columns[DatabaseConfiguration.MappingColumns.COL_CONTAINER];
         col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.Mapping.COL_CONTAINER;
         col.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
         var containerComboBoxEditor = new RepositoryItemImageComboBox();
         var smallImages = new ImageCollection();
         containerComboBoxEditor.SmallImages = smallImages;
         containerComboBoxEditor.AllowNullInput = DefaultBoolean.True;
         containerComboBoxEditor.KeyDown += onContainerComboBoxEditorKeyDown;

         containerComboBoxEditor.Items.BeginUpdate();
         foreach (DataRow row in containerTable.Rows)
         {
            var container = (string) row[ColumnNamesOfTransferTable.Container.ToString()];
            var displayName = (string) row[ColumnNamesOfTransferTable.DisplayName.ToString()];
            ApplicationIcon icon = ApplicationIcons.IconByName(container);
            if (icon != null && icon != ApplicationIcons.EmptyIcon)
            {
               smallImages.AddImage(icon.ToImage(), container);
               containerComboBoxEditor.Items.Add(new ImageComboBoxItem(displayName, container, smallImages.Images.Count - 1));
            }
            else if (container.Length > 0)
               containerComboBoxEditor.Items.Add(new ImageComboBoxItem(displayName, container));
         }
         containerComboBoxEditor.Items.EndUpdate();
         col.ColumnEdit = containerComboBoxEditor;

         col = view.Columns[DatabaseConfiguration.MappingColumns.COL_TISSUE];
         col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.Mapping.COL_TISSUE;
         col.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
         var tissueComboBoxEditor = new UxRepositoryItemComboBox(view);
         tissueComboBoxEditor.AutoComplete = true;
         tissueComboBoxEditor.AllowNullInput = DefaultBoolean.True;
         tissueComboBoxEditor.KeyDown += onTissueComboBoxEditorKeyDown;
         tissueComboBoxEditor.TextEditStyle = TextEditStyles.DisableTextEditor;
         tissueComboBoxEditor.Items.BeginUpdate();
         tissueComboBoxEditor.Items.AddRange(tissueLov);
         tissueComboBoxEditor.Items.EndUpdate();
         col.ColumnEdit = tissueComboBoxEditor;
      }

      private void onContainerComboBoxEditorKeyDown(object sender, KeyEventArgs e)
      {
         suppressKey(sender as ComboBoxEdit, e);
      }

      private void onTissueComboBoxEditorKeyDown(object sender, KeyEventArgs e)
      {
         suppressKey(sender as ComboBoxEdit, e);
      }

      private void suppressKey(BaseEdit editor, KeyEventArgs e)
      {
         if (editor == null) return;
         if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
         {
            editor.EditValue = DBNull.Value;
            e.SuppressKeyPress = true;
            e.Handled = true;
         }
      }

      /// <summary>
      ///   This event handler mask all changed cells in the mapping view with a different backround color.
      /// </summary>
      private static void onRowCellStyle(object sender, RowCellStyleEventArgs e)
      {
         var view = sender as GridView;
         if (view == null) return;

         DataRow row = view.GetDataRow(e.RowHandle);
         if (row == null) return;

         if (row[e.Column.FieldName] == DBNull.Value || row[e.Column.FieldName] == null)
         {
            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
            e.Appearance.ForeColor = Color.DimGray;
            e.Appearance.BackColor = Color.LightGray;
            e.Appearance.BackColor2 = Color.White;
         }
         if (row.RowState == DataRowState.Unchanged) return;
         
         if (e.Column.UnboundType == UnboundColumnType.Bound && row.HasVersion(DataRowVersion.Original))
         {
            if (!Equals(row[e.Column.FieldName, DataRowVersion.Current], row[e.Column.FieldName, DataRowVersion.Original]))
            {
               e.Appearance.BackColor2 = Color.LightSalmon;
            }
         }
         else if (row.RowState == DataRowState.Added)
         {
            e.Appearance.BackColor2 = Color.LightSalmon;
         }
      }

      /// <summary>
      ///   This event handler is needed because the image combo box does not allow values which are not in the combo box list.
      ///   The event handler shows the real value of the data as display text if there is no image value in the image combo box.
      /// </summary>
      private static void OnCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
      {
         if (e.Column.FieldName == DatabaseConfiguration.MappingColumns.COL_CONTAINER)
         {
            var comboBox = e.Column.ColumnEdit as RepositoryItemImageComboBox;
            if (comboBox != null)
               if (comboBox.Items.GetItem(e.Value) == null)
               {
                  if (e.Value == DBNull.Value || e.Value == null)
                     e.DisplayText = "<empty>"; //string.Empty;
                  else
                     e.DisplayText = e.Value.ToString();
               }
         }
         else if (e.Column.FieldName == DatabaseConfiguration.MappingColumns.COL_TISSUE)
         {
            var comboBox = e.Column.ColumnEdit as RepositoryItemComboBox;
            if (comboBox != null)
               if (!comboBox.Items.Contains(e.Value))
               {
                  if (e.Value == DBNull.Value || e.Value == null)
                     e.DisplayText = "<empty>";
                  else
                     e.DisplayText = e.Value.ToString();
               }
         }
      }

      /// <summary>
      ///   This event handler mask all cells with special coloring if the cell value is not present in the combo box list of values for that column.
      /// </summary>
      private static void OnCustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
      {
         if (e.CellValue == null) return;
         bool isValueInList = false;
         if (e.Column.FieldName == DatabaseConfiguration.MappingColumns.COL_CONTAINER)
         {
            var comboBox = e.Column.ColumnEdit as RepositoryItemImageComboBox;
            if (comboBox != null) isValueInList |= (comboBox.Items.GetItem(e.CellValue) != null);
         }
         else if (e.Column.FieldName == DatabaseConfiguration.MappingColumns.COL_TISSUE)
         {
            var comboBox = e.Column.ColumnEdit as RepositoryItemComboBox;
            if (comboBox != null) isValueInList |= comboBox.Items.Contains(e.CellValue);
         }
         isValueInList |= (e.CellValue.ToString().Length == 0);

         e.Appearance.BeginUpdate();
         if (!isValueInList)
         {
            e.Appearance.ForeColor = Color.Blue;
            e.Appearance.BackColor2 = Color.LightYellow;
         }
         e.Appearance.EndUpdate();
      }

      /// <summary>
      ///   This event resets value to the original value by right mouse button click.
      /// </summary>
      private static void OnRowCellClick(object sender, RowCellClickEventArgs e)
      {
         if (e.Button != MouseButtons.Right) return;
         var view = sender as GridView;
         if (view != null)
         {
            DataRow row = view.GetDataRow(e.RowHandle);
            if (row != null)
               if (row.RowState != DataRowState.Unchanged)
               {
                  if (e.Column.UnboundType == UnboundColumnType.Bound && row.HasVersion(DataRowVersion.Original))
                  {
                     if (!Equals(row[e.Column.FieldName, DataRowVersion.Current], row[e.Column.FieldName, DataRowVersion.Original]))
                     {
                        row[e.Column.FieldName] = row[e.Column.FieldName, DataRowVersion.Original];
                     }
                  }
               }
         }
      }
   }
}