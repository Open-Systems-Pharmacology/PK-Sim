using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IObservedDataInComparisonTask
   {
      /// <summary>
      ///    Returns the observed data repositories referenced by <paramref name="simulation" />, resolved against
      ///    the current project.
      /// </summary>
      IReadOnlyList<DataRepository> ResolveObservedDataFor(Simulation simulation);

      /// <summary>
      ///    Adds the observed data referenced by all simulations in <paramref name="comparison" />
      ///    to <paramref name="chart" />, then templates the per-column curve options from each
      ///    simulation's own time profile analyses so the comparison adopts the user's styling.
      /// </summary>
      void AddObservedDataToTimeProfile(PopulationSimulationComparison comparison, TimeProfileAnalysisChart chart);

      /// <summary>
      ///    Applies the curve options from <paramref name="simulation" />'s own charts to any matching
      ///    observation curves that already exist on <paramref name="chart" />. The caller is responsible
      ///    for registering the observed data on the chart and creating the curves before calling this.
      /// </summary>
      void ApplySourceCurveOptionsTo(IndividualSimulationComparison chart, IndividualSimulation simulation);
   }

   public class ObservedDataInComparisonTask : IObservedDataInComparisonTask
   {
      private readonly IExecutionContext _executionContext;

      public ObservedDataInComparisonTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public IReadOnlyList<DataRepository> ResolveObservedDataFor(Simulation simulation)
      {
         var project = _executionContext.CurrentProject;
         return simulation.UsedObservedData
            .Select(x => project.ObservedDataBy(x.Id))
            .ToList();
      }

      public void AddObservedDataToTimeProfile(PopulationSimulationComparison comparison, TimeProfileAnalysisChart chart)
      {
         var allObservedData = comparison.AllBaseSimulations
            .SelectMany(ResolveObservedDataFor)
            .Distinct()
            .ToList();

         allObservedData.Each(chart.AddObservedData);
         applyTemplatedObservationCurveOptions(comparison, chart, allObservedData);
      }

      public void ApplySourceCurveOptionsTo(IndividualSimulationComparison chart, IndividualSimulation simulation)
      {
         var sourceCurves = simulation.Charts.SelectMany(c => c.Curves).ToList();
         foreach (var observation in ResolveObservedDataFor(simulation).SelectMany(x => x.ObservationColumns()))
         {
            var sourceCurve = sourceCurves.FirstOrDefault(c => c.PlotsColumn(observation));
            if (sourceCurve == null) continue;
            chart.FindCurveWithSameData(observation.BaseGrid, observation)?.CurveOptions.UpdateFrom(sourceCurve.CurveOptions);
         }
      }

      private static void applyTemplatedObservationCurveOptions(ISimulationComparison comparison, TimeProfileAnalysisChart chart, IReadOnlyList<DataRepository> observedData)
      {
         var sourceCurveOptions = comparison.AllBaseSimulations
            .SelectMany(sim => sim.AnalysesOfType<TimeProfileAnalysisChart>())
            .SelectMany(timeProfile => timeProfile.ObservedDataCollection.ObservedDataCurveOptions())
            .ToList();

         foreach (var observation in observedData.SelectMany(x => x.ObservationColumns()))
         {
            var sourceOptions = sourceCurveOptions.FirstOrDefault(o => o.ColumnId == observation.Id);
            if (sourceOptions == null) continue;
            chart.CurveOptionsFor(observation).UpdateFrom(sourceOptions.CurveOptions);
         }
      }
   }
}
