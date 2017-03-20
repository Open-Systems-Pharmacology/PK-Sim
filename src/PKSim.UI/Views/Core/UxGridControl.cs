using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace PKSim.UI.Views.Core
{
   public class UxGridControl : GridControl
   {
      public GridView GridView
      {
         get { return MainView as GridView; }
      }
   }
}