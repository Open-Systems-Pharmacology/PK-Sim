using System.Text;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Extensions;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;

namespace PKSim.Infrastructure.Reporting.TeX
{
   internal static class StringBuilderExtensions
   {
      public static void AddIs(this StringBuilder sb, string name, object value)
      {
         sb.AppendLine(PKSimConstants.UI.ReportIs(name, value));
      }
   }

   public static class BuildTrackerExtensions
   {
      public static StructureElement CreateRelativeStructureElement(this BuildTracker tracker, string structureElementName, int level = 1)
      {
         return tracker.GetStructureElementRelativeToLast(structureElementName, level);
      }
   }

   public static class TEXBuilderExtensions
   {
      public static string ReportValue<T>(this OSPSuiteTeXBuilder<T> builder, string caption, object value)
      {
         return PKSimConstants.UI.ReportIs(caption, value).AsFullLine();
      }
   }
}