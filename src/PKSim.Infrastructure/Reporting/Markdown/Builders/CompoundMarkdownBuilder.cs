using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CompoundMarkdownBuilder : MarkdownBuilder<Compound>
   {
      private readonly IMarkdownBuilderRepository _markdownBuilderRepository;

      public CompoundMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository)
      {
         _markdownBuilderRepository = markdownBuilderRepository;
      }

      public override void Report(Compound compound, MarkdownTracker tracker)
      {
         tracker.Add(compound.Name.ToMarkdownTitle());

         var allDefaultAlternatives = compound.AllParameterAlternativeGroups().Select(x => x.DefaultAlternative).SelectMany(x => x.AllParameters()).Where(x => !x.IsDefault);

         _markdownBuilderRepository.Report(allDefaultAlternatives, tracker);
      }
   }
}