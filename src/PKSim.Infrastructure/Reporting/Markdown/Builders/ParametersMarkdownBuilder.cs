using System.Collections.Generic;
using System.Linq;
using MarkdownLog;
using OSPSuite.Core.Domain;
using PKSim.Infrastructure.Reporting.Markdown.Elements;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class ParametersMarkdownBuilder : MarkdownBuilder<IEnumerable<IParameter>>
   {
      public override void Report(IEnumerable<IParameter> parameters, MarkdownTracker tracker, int indentationLevel = 0)
      {
         var table = parameters.Select(x => x.To<ParameterElement>());
         tracker.Add(table.ToMarkdownTable());
      }
   }
}