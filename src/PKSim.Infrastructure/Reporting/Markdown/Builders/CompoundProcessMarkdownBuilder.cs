using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CompoundProcessMarkdownBuilder : MarkdownBuilder<CompoundProcess>
   {
      private readonly IMarkdownBuilderRepository _markdownBuilderRepository;

      public CompoundProcessMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository)
      {
         _markdownBuilderRepository = markdownBuilderRepository;
      }

      public override void Report(CompoundProcess compoundProcess, MarkdownTracker tracker, int indentationLevel)
      {
         tracker.Add($"{_markdownBuilderRepository.TypeFor(compoundProcess)}: {compoundProcess.Name}".ToMarkdownLevelElement(indentationLevel));
         reportSpecies(compoundProcess, tracker);
         switch (compoundProcess)
         {
            case PartialProcess partialProcess:
               report(partialProcess, tracker);
               break;
         }

         _markdownBuilderRepository.Report(compoundProcess.AllUserDefinedParameters(), tracker, indentationLevel + 1);
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