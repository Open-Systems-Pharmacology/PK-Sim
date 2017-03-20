using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class SimulationComparisonsReporter : OSPSuiteTeXReporter<IReadOnlyCollection<ISimulationComparison>>
   {
      private readonly IOSPSuiteTeXReporterRepository _reporterRepository;

      public SimulationComparisonsReporter(IOSPSuiteTeXReporterRepository reporterRepository)
      {
         _reporterRepository = reporterRepository;
      }

      public override IReadOnlyCollection<object> Report(IReadOnlyCollection<ISimulationComparison> simulationComparisons, OSPSuiteTracker buildTracker)
      {
         var list = new List<object>();

         if (!simulationComparisons.Any())
            return list;

         list.Add(new Part(PKSimConstants.UI.SimulationComparison.Pluralize()));
         simulationComparisons.Each(comparison =>
         {
            var reporter = _reporterRepository.ReportFor(comparison);
            list.AddRange(reporter.Report(comparison, buildTracker));
         });

         return list;
      }
   }
}