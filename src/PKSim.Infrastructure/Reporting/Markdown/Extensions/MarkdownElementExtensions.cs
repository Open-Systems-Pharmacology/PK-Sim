using PKSim.Infrastructure.Reporting.Markdown.Elements;

namespace PKSim.Infrastructure.Reporting.Markdown.Extensions
{
   public static class MarkdownElementExtensions
   {
      public static TitleElement ToMarkdownTitle(this string text) => new TitleElement(text);
   }
}