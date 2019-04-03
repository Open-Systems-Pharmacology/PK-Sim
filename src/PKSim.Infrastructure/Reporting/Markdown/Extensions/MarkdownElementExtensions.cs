using MarkdownLog;
using PKSim.Infrastructure.Reporting.Markdown.Elements;

namespace PKSim.Infrastructure.Reporting.Markdown.Extensions
{
   public static class MarkdownElementExtensions
   {
      public static MarkdownElement ToMarkdownTitle(this string text) => new TitleElement(text);

      public static MarkdownElement ToMarkdownSubTitle(this string text) => new SubTitleElement(text);
   }
}