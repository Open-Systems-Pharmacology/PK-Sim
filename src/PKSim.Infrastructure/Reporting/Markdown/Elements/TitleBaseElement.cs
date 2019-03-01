using System;
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

      public override string ToMarkdown() => $"{new string('#', _level)} {_text}\n";
   }
}