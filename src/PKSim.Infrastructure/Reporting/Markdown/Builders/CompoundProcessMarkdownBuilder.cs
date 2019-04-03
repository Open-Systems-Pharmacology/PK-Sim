using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CompoundProcessMarkdownBuilder : MarkdownBuilder<CompoundProcess>
   {
      private readonly IMarkdownBuilderRepository _markdownBuilderRepository;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public CompoundProcessMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IObjectTypeResolver objectTypeResolver)
      {
         _markdownBuilderRepository = markdownBuilderRepository;
         _objectTypeResolver = objectTypeResolver;
      }

      public override void Report(CompoundProcess compoundProcess, MarkdownTracker tracker, int indentationLevel)
      {
         tracker.Add($"{_objectTypeResolver.TypeFor(compoundProcess)}: {compoundProcess.Name}".ToMarkdownLevelElement(indentationLevel));
         reportSpecies(compoundProcess, tracker);
         switch (compoundProcess)
         {
            case PartialProcess partialProcess:
               report(partialProcess, tracker);
               break;
         }

         tracker.Add(PKSimConstants.ObjectTypes.Parameter.Pluralize().ToMarkdownLevelElement(indentationLevel + 1));
         _markdownBuilderRepository.Report(compoundProcess.AllParameters(x => !x.IsDefault), tracker, indentationLevel);
      }

      private void report(PartialProcess partialProcess, MarkdownTracker tracker)
      {
         tracker.AddValue(PKSimConstants.UI.Molecule, partialProcess.MoleculeName);
         switch (partialProcess)
         {
            case EnzymaticProcess enzymaticProcess when !string.IsNullOrEmpty(enzymaticProcess.MetaboliteName):
               tracker.AddValue(PKSimConstants.UI.Metabolite, enzymaticProcess.MetaboliteName);
               break;
         }
      }

      private void reportSpecies(CompoundProcess compoundProcess, MarkdownTracker tracker)
      {
         if (compoundProcess is ISpeciesDependentCompoundProcess processWithSpecies)
            tracker.AddValue(PKSimConstants.UI.Species, processWithSpecies.Species.DisplayName);
      }
   }
}