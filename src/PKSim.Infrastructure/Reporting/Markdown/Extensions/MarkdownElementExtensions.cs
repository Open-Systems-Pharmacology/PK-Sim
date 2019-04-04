using MarkdownLog;
using PKSim.Infrastructure.Reporting.Markdown.Elements;

namespace PKSim.Infrastructure.Reporting.Markdown.Extensions
{
   public static class MarkdownElementExtensions
   {
      public static MarkdownElement ToMarkdownTitle(this string text) => ToMarkdownLevelElement(text, 1);

      public static MarkdownElement ToMarkdownSubTitle(this string text) => ToMarkdownLevelElement(text, 2);

      public static MarkdownElement ToMarkdownLevelElement(this string text, int indentationLevel) => new TitleBaseElement(text, indentationLevel);
   }
}