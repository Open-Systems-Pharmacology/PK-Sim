using System.Linq;
using System.Text;
using MarkdownLog;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown
{
   public class MarkdownTracker
   {
      private const string OFFSET = "     ";
      private const string TABLE_OFFSET = "\r\n     ";
      private const string TABLE_OFFSET_NO_INDENT = "\r\n";
      private const string NEW_LINE = "\r\n\r\n";

      /// <summary>
      ///    The string builder containing the actual Markdown being generated
      /// </summary>
      public StringBuilder Markdown { get; } = new StringBuilder();

      public override string ToString()
      {
         return Markdown.ToString();
      }

      public MarkdownTracker Add(IMarkdownElement markdownElement) => Add(markdownElement.ToString());

      public MarkdownTracker Add(Table markdownTable) => Add(adjustTableHeader(markdownTable).ToString().Remove(0, OFFSET.Length).Replace(TABLE_OFFSET, TABLE_OFFSET_NO_INDENT));

      public MarkdownTracker Add(string rawMarkdown)
      {
         Markdown.Append(rawMarkdown);
         return this;
      }

      public MarkdownTracker AddValue(string caption, string value) => Add($"{caption}: {value}{NEW_LINE}");

      private Table adjustTableHeader(Table table)
      {
         var headers = table.Columns.Select(x => x.HeaderCell.DowncastTo<TableCell>().Text);
         return table.WithHeaders(headers.Select(x => x.SplitToUpperCase()).ToArray());
      }
   }
}