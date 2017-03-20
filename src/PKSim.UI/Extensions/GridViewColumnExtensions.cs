using OSPSuite.DataBinding.DevExpress.XtraGrid;

namespace PKSim.UI.Extensions
{
   public static class GridViewColumnExtensions
   {
      /// <summary>
      /// Set the visibility of the underlying XtraColumn defined in the <paramref name="column"/> and updates its visibility in the ColumnChooser accordingly
      /// </summary>
      /// <param name="column">Grid view column</param>
      /// <param name="visible">Is the column visible?</param>
       public static void UpdateVisibility(this IGridViewColumn column, bool visible)
       {
          column.Visible = visible;
          column.WithShowInColumnChooser(visible);
       }
   }
}