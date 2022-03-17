using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Extensions
{
   public static class TablePanelExtensions
   {
      public static TablePanel WithAbsoluteWidth(this TablePanel tablePanel, int columnIndex0, int width)
      {
         tablePanel.Columns[columnIndex0].Style = TablePanelEntityStyle.Absolute;
         tablePanel.Columns[columnIndex0].Width = width;
         return tablePanel;
      }

      public static void AdjustButtonWithImageOnly(this SimpleButton button)
      {
         button.Size = new Size(OSPSuite.UI.UIConstants.Size.RADIO_GROUP_HEIGHT, OSPSuite.UI.UIConstants.Size.BUTTON_HEIGHT);
      }

      public static void AdjustLongButtonWidth(this SimpleButton button)
      {
         button.Size = new Size(OSPSuite.UI.UIConstants.Size.LARGE_BUTTON_WIDTH, button.Height);
      }

      public static TablePanelRow RowFor(this TablePanel tablePanel, Control control)
      {
         return tablePanel.Rows[tablePanel.GetRow(control)];
      }
   }
}