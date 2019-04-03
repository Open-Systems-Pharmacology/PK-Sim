using System.Collections.Generic;
using System.Linq;
using MarkdownLog;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
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
         tracker.Add($"{PKSimConstants.ObjectTypes.Compound}: {compound.Name}".ToMarkdownTitle());
         tracker.Add(PKSimConstants.ObjectTypes.Parameter.Pluralize().ToMarkdownSubTitle());


         var allAlternatives = compound.AllParameterAlternativeGroups().SelectMany(x => x.AllAlternatives);
         var allParametersAlternatives = new List<ParameterAlternative>();
         foreach (var alternative in allAlternatives)
         {
            var allUserDefinedParameters = alternative.AllParameters(p => !p.IsDefault).ToList();
            allUserDefinedParameters.Each(p =>
            {
               allParametersAlternatives.Add(new ParameterAlternative
               {
                  Alternative = alternative.Name,
                  Default = alternative.IsDefault,
                  Name = p.Name,
                  ValueOrigin = p.ValueOrigin.ToString(),
                  Value = $"{p.ValueInDisplayUnit} {p.DisplayUnit.Name}"
               });
            });
         }

         tracker.Add(allParametersAlternatives.ToMarkdownTable());
      }

      private class ParameterAlternative
      {
         public string Name { get; set; }
         public string Value { get; set; }
         public string ValueOrigin { get; set; }
         public string Alternative { get; set; }
         public bool Default { get; set; }
      }
   }
}