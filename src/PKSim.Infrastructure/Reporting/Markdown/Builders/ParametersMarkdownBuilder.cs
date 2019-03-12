using System.Collections.Generic;
using System.Linq;
using MarkdownLog;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class ParametersMarkdownBuilder : MarkdownBuilder<IEnumerable<IParameter>>
   {
      public override void Report(IEnumerable<IParameter> parameters, MarkdownTracker tracker)
      {
         var table = parameters.Select(x => new
         {
            Name = x.Name,
            Value = x.ValueInDisplayUnit,
            Unit = x.DisplayUnit.ToString(),
         });

         tracker.Add(table.ToMarkdownTable());
      }
   }
}
