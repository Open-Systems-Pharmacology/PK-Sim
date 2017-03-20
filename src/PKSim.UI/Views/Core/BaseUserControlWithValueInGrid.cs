using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;

namespace PKSim.UI.Views.Core
{
   public partial class BaseUserControlWithValueInGrid : BaseContainerUserControl
   {
      public BaseUserControlWithValueInGrid()
      {
         InitializeComponent();
      }

      protected void InitializeWithGrid(UxGridView gridView)
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

         if (!ColumnIsValue(col))
         {
            gridView.EditorShowMode = EditorShowMode.Default;
            return;
         }

         OnValueColumnMouseDown(gridView, col, rowHandle);
      }

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