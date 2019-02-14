using System.Collections.Generic;
using System.Text;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Reporting
{
   public class ReportPart
   {
      private readonly IList<ReportPart> _subParts;
      private readonly StringBuilder _sbContent;
      private string _title;

      public string Title
      {
         get => _title;
         set
         {
            _title = value;
            if (string.IsNullOrEmpty(_title))
               return;
            _title = _title.Replace("&&", "&");
         }
      }

      public ReportPart()
      {
         _subParts = new List<ReportPart>();
         _sbContent = new StringBuilder();
         _title = string.Empty;
      }

      /// <summary>
      ///    Add line to content. If line does not end with '\n', it is added at the end
      /// </summary>
      /// <param name="lineToAdd">Line to add as a string. it can contain special format character such as {0} that should then set in the args list</param>
      /// <param name="args"></param>
      public void AddToContent(string lineToAdd, params object[] args)
      {
         if (string.IsNullOrEmpty(lineToAdd)) return;
         if (!lineToAdd.EndsWith("\n"))
            lineToAdd += "\n";
         _sbContent.AppendFormat(lineToAdd, args);
      }

      /// <summary>
      ///    Add the report as string to the content of the current report part
      /// </summary>
      public void AddToContent(ReportPart reportPart)
      {
         AddToContent(reportPart.ToStringReport());
      }

      /// <summary>
      ///    Add a sub part to the report
      /// </summary>
      public void AddPart(ReportPart reportPart)
      {
         _subParts.Add(reportPart);
      }

      /// <summary>
      ///    Returns the content of the report. Sub parts content is not included
      /// </summary>
      public virtual string Content => _sbContent.ToString();

      public IEnumerable<ReportPart> SubParts => _subParts;

      public override string ToString()
      {
         return Title;
      }

      /// <summary>
      ///    Returns title, content + sub parts
      /// </summary>
      public string ToStringReport()
      {
         var sb = new StringBuilder();
         addPartTo(sb);
         return sb.ToString();
      }

      public StringBuilder ContentToStringBuilder()
      {
         return _sbContent;
      }

      private void addPartTo(StringBuilder sb)
      {
         if (string.IsNullOrEmpty(Content) && _subParts.Count == 0)
            return;

         if (!string.IsNullOrEmpty(Title))
            sb.AppendLine(Title);

         if (!string.IsNullOrEmpty(Content))
            sb.Append(Content);

         _subParts.Each(x => x.addPartTo(sb));
      }
   }
   public static class ReportPartExtensions
   {
      public static T WithTitle<T>(this T reportPart, string title) where T : ReportPart
      {
         reportPart.Title = title;
         return reportPart;
      }
   }
}