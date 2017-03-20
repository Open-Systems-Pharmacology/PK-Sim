using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   internal class SimulationAnalysisReporter : OSPSuiteTeXReporter<ISimulationAnalysis>
   {
      private readonly IObjectTypeResolver _objectTypeResolver;

      public SimulationAnalysisReporter(IObjectTypeResolver objectTypeResolver)
      {
         _objectTypeResolver = objectTypeResolver;
      }

      public override IReadOnlyCollection<object> Report(ISimulationAnalysis analysis, OSPSuiteTracker buildTracker)
      {
         var listToReport = new List<object>();
         listToReport.Add(new Chapter(_objectTypeResolver.TypeFor(analysis)));
         listToReport.Add(new Section(analysis.Name));
         listToReport.Add(analysis);

         return listToReport;
      }
   }
}