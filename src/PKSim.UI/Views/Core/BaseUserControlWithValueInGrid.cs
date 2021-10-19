using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class BaseUserControlWithValueInGrid : BaseContainerUserControl
   {
      public BaseUserControlWithValueInGrid()
      {
         InitializeComponent();
      }

      protected virtual void InitializeWithGrid(UxGridView gridView)
      {
         gridView.MouseDown += (sender, e) => OnGridViewMouseDown(gridView, e);
      }

      protected virtual void OnGridViewMouseDown(UxGridView gridView, MouseEventArgs e)
      {
         // Check if the end-user has right clicked the grid control. 
         if (e.Button != MouseButtons.Left) return;

         //valid hit test?
         var col = gridView.ColumnAt(e);
         if (col == null) return;

         var rowHandle = gridView.RowHandleAt(e);
         if (rowHandle < 0) return;

         gridView.PostEditor();

         if (ColumnIsButton(col))
         {
            gridView.RaiseButtonClick(e);
            return;
         }

         if (ColumnIsCheckBox(col))
         {
            RaiseCheckEditClick(gridView, col, rowHandle, e);
            return;
         }

         if (!ColumnIsValue(col))
         {
            gridView.EditorShowMode = EditorShowMode.Default;
            return;
         }

         OnValueColumnMouseDown(gridView, col, rowHandle);
      }

      protected virtual void RaiseCheckEditClick(UxGridView gridView, GridColumn column, int rowHandle, MouseEventArgs e)
      {
         //Adapted from https://supportcenter.devexpress.com/ticket/details/t230842/grid-the-buttonclick-event-is-not-raised-immediately-when-multi-selection-is-enabled
         gridView.FocusedRowHandle = rowHandle;
         gridView.FocusedColumn = column;
         gridView.ShowEditor();
         //force button click  
         var edit = gridView.ActiveEditor.DowncastTo<CheckEdit>();
         edit.Toggle();
         ((DXMouseEventArgs) e).Handled = true;
      }

      protected virtual bool ColumnIsButton(GridColumn column) => false;

      protected virtual bool ColumnIsCheckBox(GridColumn column) => false;

      protected virtual void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         gridView.EditorShowMode = EditorShowMode.MouseUp;
      }

      protected virtual bool ColumnIsValue(GridColumn column)
      {
         return false;
      }
   }
}