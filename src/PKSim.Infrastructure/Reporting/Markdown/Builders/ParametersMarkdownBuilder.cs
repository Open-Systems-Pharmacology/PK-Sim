using System.Collections.Generic;
using System.Linq;
using MarkdownLog;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;
using PKSim.Infrastructure.Reporting.Markdown.Mappers;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class ParametersMarkdownBuilder : MarkdownBuilder<IEnumerable<IParameter>>
   {
      private readonly IParameterToParameterElementMapper _parameterElementMapper;

      public ParametersMarkdownBuilder(IParameterToParameterElementMapper parameterElementMapper)
      {
         _parameterElementMapper = parameterElementMapper;
      }

      public override void Report(IEnumerable<IParameter> parameters, MarkdownTracker tracker, int indentationLevel)
      {
         var parameterElements = parameters.Select(x => _parameterElementMapper.MapFrom(x)).ToList();
         if (!parameterElements.Any())
            return;

         tracker.Add(PKSimConstants.ObjectTypes.Parameter.Pluralize().ToMarkdownLevelElement(indentationLevel));
         tracker.Add(parameterElements.ToMarkdownTable());
      }
   }
}