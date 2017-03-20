using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class SimulationAnalysesTeXBuilder : OSPSuiteTeXBuilder<IReadOnlyList<ISimulationAnalysis>>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public SimulationAnalysesTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(IReadOnlyList<ISimulationAnalysis> analyses, OSPSuiteTracker buildTracker)
      {
         var report = new List<object>();

         // add population result charts
         var charts = analyses.OfType<SimulationTimeProfileChart>().ToArray();
         if (charts.Any())
         {
            report.Add(new Section(PKSimConstants.UI.Charts));
            foreach (var chart in charts)
            {
               if (!chart.Curves.Any())
                  continue;

               report.Add(new SubSection(chart.Name));
               report.Add(chart);
               report.Add(new IndividualPKAnalyses(chart.Analysable.DowncastTo<Simulation>(), chart));
            }
         }

         addAnalysesReports<TimeProfileAnalysisChart>(analyses, PKSimConstants.UI.TimeProfileAnalysis, report);
         addAnalysesReports<BoxWhiskerAnalysisChart>(analyses, PKSimConstants.UI.BoxWhiskerAnalysis, report);
         addAnalysesReports<ScatterAnalysisChart>(analyses, PKSimConstants.UI.ScatterAnalysis, report);
         addAnalysesReports<RangeAnalysisChart>(analyses, PKSimConstants.UI.RangeAnalysis, report);

         _builderRepository.Report(report, buildTracker);
      }

      private void addAnalysesReports<TAnalysis>(IEnumerable<ISimulationAnalysis> analyses, string sectionTitle, List<object> report) where TAnalysis : ISimulationAnalysis
      {
         // add range analysis charts
         var analysesOfType = analyses.OfType<TAnalysis>().ToList();
         if (!analysesOfType.Any())
            return;

         report.Add(new Section(sectionTitle));
         foreach (var analysis in analysesOfType)
         {
            report.Add(new SubSection(analysis.Name));
            report.Add(analysis);
         }
      }
   }
}