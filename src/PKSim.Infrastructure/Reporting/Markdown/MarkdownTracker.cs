using System.Text;
using MarkdownLog;

namespace PKSim.Infrastructure.Reporting.Markdown
{
   public class MarkdownTracker
   {

      /// <summary>
      ///    The string builder containing the actual Markdown being generated
      /// </summary>
      /// 
      public StringBuilder Markdown { get;  } = new StringBuilder();

      public MarkdownTracker()
      {
      }

      public override string ToString()
      {
         return Markdown.ToString();
      }

      public MarkdownTracker Add(IMarkdownElement markdown)
      {
         Markdown.Append(markdown.ToMarkdown());
         return this;
      }
   }
}