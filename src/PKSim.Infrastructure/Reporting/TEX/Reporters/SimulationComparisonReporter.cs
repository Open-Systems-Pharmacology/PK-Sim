using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public abstract class SimulationComparisonReporter<TSimulationComparison> : OSPSuiteTeXReporter<TSimulationComparison> where TSimulationComparison : class, ISimulationComparison
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      protected SimulationComparisonReporter(ILazyLoadTask lazyLoadTask)
      {
         _lazyLoadTask = lazyLoadTask;
      }

      public override IReadOnlyCollection<object> Report(TSimulationComparison simulationComparison, OSPSuiteTracker buildTracker)
      {
         _lazyLoadTask.Load(simulationComparison);
         var chapter = new Chapter(simulationComparison.Name);
         var report = new List<object> {chapter};
         buildTracker.AddReference(simulationComparison, chapter);

         var populationComparison = simulationComparison as PopulationSimulationComparison;
         if (populationComparison != null)
         {
            report.Add(getTableFor(populationComparison));

            if (populationComparison.ReferenceGroupingItem != null)
               report.Add(new Text("The reference simulation is named: {0}", populationComparison.ReferenceGroupingItem.Label));

            report.Add(new SelectedDistribution(populationComparison, 1));

            if (populationComparison.Analyses.Any())
               report.Add(populationComparison.Analyses.ToList());
         }
         var comparison = simulationComparison as IndividualSimulationComparison;
         if (comparison != null)
            report.Add(comparison);

         return report;
      }

      private DataTable getTableFor(PopulationSimulationComparison populationSimulationComparison)
      {
         var dt = new DataTable(PKSimConstants.UI.SimulationsUsedInComparison);

         dt.AddColumn(PKSimConstants.ObjectTypes.Simulation);
         if (populationSimulationComparison.ReferenceSimulation != null)
            dt.AddColumn<bool>(PKSimConstants.UI.ReferenceSimulation);

         dt.BeginLoadData();
         foreach (var simulation in populationSimulationComparison.AllSimulations)
         {
            var row = dt.NewRow();
            row[PKSimConstants.ObjectTypes.Simulation] = simulation.Name;
            if (populationSimulationComparison.ReferenceSimulation != null)
               row[PKSimConstants.UI.ReferenceSimulation] = (simulation.Name == populationSimulationComparison.ReferenceSimulation.Name);
            dt.Rows.Add(row);
         }
         dt.EndLoadData();

         return dt;
      }
   }

   public class PopulationSimulationComparisonReporter : SimulationComparisonReporter<PopulationSimulationComparison>
   {
      public PopulationSimulationComparisonReporter(ILazyLoadTask lazyLoadTask)
         : base(lazyLoadTask)
      {
      }
   }

   public class IndividualSimulationComparisonReporter : SimulationComparisonReporter<IndividualSimulationComparison>
   {
      public IndividualSimulationComparisonReporter(ILazyLoadTask lazyLoadTask)
         : base(lazyLoadTask)
      {
      }
   }
}