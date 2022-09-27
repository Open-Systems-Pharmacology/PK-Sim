using MarkdownLog;

namespace PKSim.Infrastructure.Reporting.Markdown.Elements
{
   public class TitleBaseElement : MarkdownElement
   {
      private readonly string _text;
      private readonly int _level;

      public TitleBaseElement(string text, int level)
      {
         _text = text;
         _level = level;
      }

      //Ensure that we always have a new line before a heading and after
      public override string ToMarkdown() => $"{new string('#', _level)} {_text}\n\n";
   }
}