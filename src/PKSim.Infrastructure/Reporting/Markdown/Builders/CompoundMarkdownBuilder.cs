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
using PKSim.Infrastructure.Reporting.Markdown.Mappers;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CompoundMarkdownBuilder : BuildingBlockMarkdownBuilder<Compound>
   {
      private readonly IParameterToParameterElementMapper _parameterElementMapper;

      public CompoundMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IParameterToParameterElementMapper parameterElementMapper) : base(markdownBuilderRepository)
      {
         _parameterElementMapper = parameterElementMapper;
      }

      public override void Report(Compound compound, MarkdownTracker tracker, int indentationLevel)
      {
         base.Report(compound, tracker, indentationLevel);
         var subLevelIndentation = indentationLevel + 1;
         tracker.Add(PKSimConstants.ObjectTypes.Parameter.Pluralize().ToMarkdownLevelElement(subLevelIndentation));

         var allAlternatives = compound.AllParameterAlternativeGroups().SelectMany(x => x.AllAlternatives);
         var allCompoundParameters = new List<CompoundParameter>();
         foreach (var alternative in allAlternatives)
         {
            var allUserDefinedParameters = alternative.AllUserDefinedParameters().ToList();
            allUserDefinedParameters.Each(p => allCompoundParameters.Add(mapFrom(alternative, p)));
         }

         var allSimpleParameters = compound.AllSimpleParameters().Where(p => !p.IsDefault).ToList();
         allSimpleParameters.Each(p => allCompoundParameters.Add(mapFrom(p)));
         tracker.Add(allCompoundParameters.ToMarkdownTable());

         tracker.Add(PKSimConstants.UI.CalculationMethods.ToMarkdownLevelElement(subLevelIndentation));
         _markdownBuilderRepository.Report(compound.CalculationMethodCache, tracker);

         tracker.Add(PKSimConstants.UI.Processes.ToMarkdownLevelElement(subLevelIndentation));
         compound.AllProcesses().Each(x => _markdownBuilderRepository.Report(x, tracker, subLevelIndentation + 1));
      }

      private CompoundParameter mapFrom(IParameter parameter) => _parameterElementMapper.MapFrom<CompoundParameter>(parameter);

      private CompoundParameter mapFrom(ParameterAlternative alternative, IParameter parameter)
      {
         return _parameterElementMapper.MapFrom<CompoundParameter>(parameter, compoundParameter =>
         {
            compoundParameter.Alternative = alternative.Name;
            compoundParameter.Default = alternative.IsDefault;
         });
      }

      private class CompoundParameter : IParameterElement
      {
         public string Name { get; set; }
         public string Value { get; set; }
         public ValueOrigin ValueOrigin { get; set; }
         public string Alternative { get; set; }
         public bool? Default { get; set; }
      }
   }
}