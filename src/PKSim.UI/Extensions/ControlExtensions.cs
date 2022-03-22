using System.Windows.Forms;

namespace PKSim.UI.Extensions
{
   public static class ControlExtensions
   {
      public static void UpdateMargin(this Control control, int? left = null, int? top = null, int? right = null, int? bottom = null)
      {
         var currentMargin = control.Margin;
         var leftToUse = left.GetValueOrDefault(currentMargin.Left);
         var topToUse = top.GetValueOrDefault(currentMargin.Top);
         var rightToUse = right.GetValueOrDefault(currentMargin.Right);
         var bottomToUse = bottom.GetValueOrDefault(currentMargin.Bottom);
         control.Margin = new Padding(leftToUse, topToUse, rightToUse, bottomToUse);
      }
   }
}
