using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using OSPSuite.UI;
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

      public static TablePanel WithLongButtonWidth(this TablePanel tablePanel, params int[] columnIndexes)
      {
         //TODO define constant
         columnIndexes.Each(columnIndex => WithAbsoluteWidth(tablePanel, columnIndex, 150));
         return tablePanel;
      }

      public static TablePanel WithButtonWidth(this TablePanel tablePanel, params int[] columnIndexes)
      {
         columnIndexes.Each(columnIndex => WithAbsoluteWidth(tablePanel, columnIndex, 30));
         return tablePanel;
      }

      public static void AdjustButtonWithImageOnly(this SimpleButton button)
      {
         button.Size = new Size(UIConstants.Size.ScaleForScreenDPI(24), UIConstants.Size.ScaleForScreenDPI(22));
      }
      public static void AdjustLongButtonWidth(this SimpleButton button)
      {
         button.Size = new Size(UIConstants.Size.LARGE_BUTTON_WIDTH, button.Height);
      }
   }
}