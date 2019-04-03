using System.Collections.Generic;
using System.Linq;
using MarkdownLog;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Elements;
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

      public override void Report(Compound compound, MarkdownTracker tracker, int indentationLevel)
      {
         tracker.Add($"{PKSimConstants.ObjectTypes.Compound}: {compound.Name}".ToMarkdownLevelElement(indentationLevel));
         var sublevelIndentation = indentationLevel + 1;
         tracker.Add(PKSimConstants.ObjectTypes.Parameter.Pluralize().ToMarkdownLevelElement(sublevelIndentation));

         var allAlternatives = compound.AllParameterAlternativeGroups().SelectMany(x => x.AllAlternatives);
         var allCompoundParameters = new List<CompoundParameter>();
         foreach (var alternative in allAlternatives)
         {
            var allUserDefinedParameters = alternative.AllParameters(p => !p.IsDefault).ToList();
            allUserDefinedParameters.Each(p => allCompoundParameters.Add(mapFrom(alternative, p)));
         }

         var allSimpleParameters = compound.AllSimpleParameters().Where(p => !p.IsDefault).ToList();
         allSimpleParameters.Each(p => allCompoundParameters.Add(mapFrom(p)));
         tracker.Add(allCompoundParameters.ToMarkdownTable());

         tracker.Add(PKSimConstants.UI.CalculationMethods.ToMarkdownLevelElement(sublevelIndentation));
         _markdownBuilderRepository.Report(compound.CalculationMethodCache, tracker);

         tracker.Add(PKSimConstants.UI.Processes.ToMarkdownLevelElement(sublevelIndentation));
         compound.AllProcesses().Each(x => _markdownBuilderRepository.Report(x, tracker, sublevelIndentation + 1));
      }

      private CompoundParameter mapFrom(IParameter parameter) => parameter.To<CompoundParameter>();

      private CompoundParameter mapFrom(ParameterAlternative alternative, IParameter parameter)
      {
         var compoundParameter = mapFrom(parameter);
         compoundParameter.Alternative = alternative.Name;
         compoundParameter.Alternative = alternative.Name;
         compoundParameter.Default = alternative.IsDefault;
         return compoundParameter;
      }

      private class CompoundParameter : IParameterElement
      {
         public string Name { get; set; }
         public string Value { get; set; }
         public string ValueOrigin { get; set; }
         public string Alternative { get; set; }
         public bool? Default { get; set; }
      }
   }
}