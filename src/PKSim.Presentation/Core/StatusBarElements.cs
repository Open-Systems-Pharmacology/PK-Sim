using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Core
{
   public static class StatusBarElements
   {
      private static readonly IList<StatusBarElement> _allElements = new List<StatusBarElement>();

      public static StatusBarElement Status = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.None, StatusBarElementAlignment.Left, 150);
      public static StatusBarElement ProjectName = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Left, 150);
      public static StatusBarElement ProjectPath = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Left, 150);
      public static StatusBarElement Journal = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Left, 150);
      public static StatusBarElement Report = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Right, 150);
      public static StatusBarElement ProgressStatus = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Right, 250);
      public static StatusBarElement ProgressBar = createStatusBarElement(StatusBarElementType.ProgressBar, StatusBarElementSize.None, StatusBarElementAlignment.Right, 150);
      public static StatusBarElement Version = createStatusBarElement(StatusBarElementType.Text, StatusBarElementSize.Content, StatusBarElementAlignment.Right, 0);

      private static StatusBarElement createStatusBarElement(StatusBarElementType elementType, StatusBarElementSize statusBarElementSize, StatusBarElementAlignment statusBarElementAlignment, int width, StringAlignment textAlignment = StringAlignment.Near)
      {
         var statusBarElement = new StatusBarElement(elementType, statusBarElementSize, statusBarElementAlignment, width, textAlignment)
         {
            Index = _allElements.Count
         };
         _allElements.Add(statusBarElement);
         return statusBarElement;
      }

      public static IEnumerable<StatusBarElement> All()
      {
         return _allElements.All();
      }
   }
}