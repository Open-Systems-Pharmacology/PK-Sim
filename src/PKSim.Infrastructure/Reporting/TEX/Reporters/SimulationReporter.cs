using System.Collections.Generic;
using System.Linq;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class SimulationReporter : OSPSuiteTeXReporter<Simulation>
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      public SimulationReporter(ILazyLoadTask lazyLoadTask)
      {
         _lazyLoadTask = lazyLoadTask;
      }

      public override IReadOnlyCollection<object> Report(Simulation simulation, OSPSuiteTracker buildTracker)
      {
         _lazyLoadTask.Load(simulation);
         var chapter = new Chapter(simulation.Name);
         var report = new List<object> {chapter};
         buildTracker.AddReference(simulation, chapter);
         report.AddRange(this.ReportDescription(simulation, buildTracker));
         report.Add(new Section(PKSimConstants.UI.UsedBuildingBlocks));
         report.Add(simulation.UsedBuildingBlocks);

         report.Add(new Section(PKSimConstants.UI.SimulationProperties));
         report.Add(simulation.Properties);

         addHistograms(simulation, report);

         if (simulation.Analyses.Any())
            report.Add(simulation.Analyses.ToList());

         return report;
      }

      private void addHistograms(Simulation simulation, List<object> report)
      {
         var populationSimulation = simulation as PopulationSimulation;
         if (populationSimulation == null) return;

         report.Add(new SelectedDistribution(populationSimulation, -3));
      }
   }
}