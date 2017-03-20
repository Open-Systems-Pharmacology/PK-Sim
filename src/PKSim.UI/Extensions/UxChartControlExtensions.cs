using PKSim.Core;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Extensions
{
   public static class UxChartControlExtensions
   {
      public static void InitializeColor(this UxChartControl uxChartControl)
      {
         uxChartControl.PaletteName = "Pastel Kit";
         uxChartControl.DiagramBackColor = PKSimColors.ChartDiagramBack;
         uxChartControl.BackColor = PKSimColors.ChartBack;
      }
   }
}