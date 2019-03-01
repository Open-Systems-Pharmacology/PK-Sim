using System.Linq;
using MarkdownLog;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CompoundMarkdownBuilder : MarkdownBuilder<Compound>
   {
      public override void Report(Compound compound, MarkdownTracker tracker)
      {
         tracker.Add(compound.Name.ToMarkdownTitle());


         var allDefaultAlternatives = compound.AllParameterAlternativeGroups().Select(x => x.DefaultAlternative).SelectMany(x => x.AllParameters()).Where(x => !x.IsDefault);

         var parameters = allDefaultAlternatives.Select(x => new
         {
            Name = x.Name,
            Value = x.ValueInDisplayUnit,
            Unit = x.DisplayUnit.ToString(),
         });

         tracker.Add(parameters.ToMarkdownTable());
      }
   }
}