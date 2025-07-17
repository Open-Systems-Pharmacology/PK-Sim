using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class SimulationPropertiesTeXBuilder : OSPSuiteTeXBuilder<SimulationProperties>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IReportGenerator _reportGenerator;

      public SimulationPropertiesTeXBuilder(ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator)
      {
         _texBuilderRepository = texBuilderRepository;
         _reportGenerator = reportGenerator;
      }

      public override void Build(SimulationProperties simulationProperties, OSPSuiteTracker buildTracker)
      {
         var objectsToReport = new List<object>();

         objectsToReport.Add(new SubSection(PKSimConstants.UI.ModelStructure));

         var part = new ReportPart { Title = PKSimConstants.UI.AllowAging };
         part.AddToContent(simulationProperties.AllowAging ? PKSimConstants.UI.Yes : PKSimConstants.UI.No);
         objectsToReport.Add(part);
         objectsToReport.Add(_reportGenerator.ReportFor(simulationProperties.ModelProperties));

         objectsToReport.Add(new SubSection(PKSimConstants.UI.SimulationCompoundsConfiguration));
         objectsToReport.AddRange(getObjectsToReport(simulationProperties, buildTracker, cp => cp));

         if (anyProcessesDefined(simulationProperties))
         {
            objectsToReport.Add(new SubSection(PKSimConstants.UI.Processes));
            objectsToReport.AddRange(getObjectsToReport(simulationProperties, buildTracker, cp => cp.Processes.Any() ? cp.Processes : null));
         }

         objectsToReport.Add(new SubSection(PKSimConstants.UI.Administration));
         objectsToReport.AddRange(getObjectsToReport(simulationProperties, buildTracker, cp => cp.ProtocolProperties.Protocol == null ? null : cp.ProtocolProperties));

         if (anyEventsDefined(simulationProperties))
         {
            objectsToReport.Add(new SubSection(PKSimConstants.UI.SimulationEventsConfiguration));
            objectsToReport.Add(simulationProperties.EventProperties);
         }

         _texBuilderRepository.Report(objectsToReport, buildTracker);
      }

      private bool anyEventsDefined(SimulationProperties simulationProperties)
      {
         return simulationProperties.EventProperties.EventMappings.Any();
      }

      private static bool anyProcessesDefined(SimulationProperties simulationProperties)
      {
         return simulationProperties.CompoundPropertiesList.Any(x => x.AnyProcessesDefined);
      }

      private static IEnumerable<object> getObjectsToReport(SimulationProperties simulationProperties, BuildTracker buildTracker, Func<CompoundProperties, object> objectSelectionFunc)
      {
         foreach (var compoundProperties in simulationProperties.CompoundPropertiesList)
         {
            var objectsToAdd = objectSelectionFunc(compoundProperties);
            if (objectsToAdd == null) continue;
            // We are creating a relative structural element two levels below for the current tracker because in the calling method, we have already added a structural element
            // that's 1 level below but the tracker has not been updated for that.
            yield return buildTracker.CreateRelativeStructureElement(compoundProperties.Compound.Name, 2);
            yield return objectsToAdd;
         }
      }
   }
}