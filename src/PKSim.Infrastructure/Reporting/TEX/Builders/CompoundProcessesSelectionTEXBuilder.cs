using System.Collections.Generic;
using System.Linq;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class CompoundProcessesSelectionTeXBuilder : OSPSuiteTeXBuilder<CompoundProcessesSelection>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public CompoundProcessesSelectionTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(CompoundProcessesSelection processes, OSPSuiteTracker buildTracker)
      {
         var thisSection = new List<object>();
         thisSection.AddRange(processSelection(processes.MetabolizationSelection, buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.SimulationMetabolism, 2)));
         thisSection.AddRange(processSelection(processes.TransportAndExcretionSelection, buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.SimulationTransportAndExcretion, 2)));
         thisSection.AddRange(processSelection(processes.SpecificBindingSelection, buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.SimulationSpecificBinding, 2)));

         _builderRepository.Report(thisSection, buildTracker);
      }

      private IEnumerable<object> processSelection(ProcessSelectionGroup processSelectionGroup, StructureElement element)
      {
         if (!processSelectionGroup.AllEnabledProcesses().Any())
            yield break;

         yield return element;
         yield return processSelectionGroup;
      }
   }
}
