using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Assets;

namespace PKSim.Core
{
   public static class PKSimColors
   {
      /// <summary>
      /// Color used for cell that are a formu;a
      /// </summary>
      public static Color Formula = Color.Azure;

      /// <summary>
      ///Color used for cells containing parameter whose value was changed by the user
      /// </summary>
      public static Color Changed = Color.LightYellow;

      /// <summary>
      /// Color used for a plot back color (everything but diagram)
      /// </summary>
      public static Color ChartBack
      {
         get { return Colors.ChartBack; }
         set { Colors.ChartBack = value; }
      }

      public static Color ChartDiagramBack
      {
         get { return Colors.ChartDiagramBack; }
         set { Colors.ChartDiagramBack = value; }
      }

      /// <summary>
      /// Color used for cell that are locked/disabled 
      /// </summary>
      public static Color Disabled = Colors.Disabled;

      /// <summary>
      /// Color used for the female gender (Start of gradient)
      /// </summary> 
      public static Color Female = Color.FromArgb(220, 99, 78);

      /// <summary>
      /// Color used for the female gender (End of gradient)
      /// </summary> 
      public static Color Female2 = Color.FromArgb(239, 130, 111);

      /// <summary>
      /// Color used for the male gender (Start of gradient)
      /// </summary> 
      public static Color Male = Colors.Blue1;

      /// <summary>
      /// Color used for the female gender (End of gradient)
      /// </summary> 
      public static Color Male2 = Colors.Blue2;

      /// <summary>
      /// Color used for the selected distribution that should be exported in report
      /// </summary>
      public static Color SelectedDistribution = Color.DodgerBlue;

      /// <summary>
      /// Color used to generate a grouping color gradient in NumberOfBins use case (Start of gradient)
      /// </summary>
      public static Color StartGroupingColor = Color.DeepSkyBlue;

      /// <summary>
      /// Color used to generate a grouping color gradient in NumberOfBins use case (End of gradient)
      /// </summary>
      public static Color EndGroupingColor = Color.Red;

      /// <summary>
      /// Default colors used for series (for instance in population analysis)
      /// </summary>
      public static IReadOnlyList<Color> DefaultSeriesColors = new List<Color>{Color.Black, Color.Red, Color.Blue, Color.Green, Color.Magenta, Color.Turquoise, Color.DarkOrange, Color.DarkOrchid};

   }
}