using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.Services
{
   public class SimulationResultsTask : ISimulationResultsTask
   {
      private readonly IChartTemplatingTask _chartTemplatingTask;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly ICloner _cloner;
      private readonly ISimulationResultsCreator _simulationResultsCreator;
      private readonly IDataRepositoryFromResultsCreator _dataRepositoryCreator;
      private readonly ICurveNamer _curveNamer;

      public SimulationResultsTask(IChartTemplatingTask chartTemplatingTask, ISimulationResultsSynchronizer simulationResultsSynchronizer, ICloner cloner, ISimulationResultsCreator simulationResultsCreator,
         IDataRepositoryFromResultsCreator dataRepositoryCreator, ICurveNamer curveNamer)
      {
         _chartTemplatingTask = chartTemplatingTask;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _cloner = cloner;
         _simulationResultsCreator = simulationResultsCreator;
         _dataRepositoryCreator = dataRepositoryCreator;
         _curveNamer = curveNamer;
      }

      public void CloneResults(Simulation sourceSimulation, Simulation targetSimulation)
      {
         if (!canCopyResults(sourceSimulation, targetSimulation)) return;

         if (targetSimulation.IsAnImplementationOf<IndividualSimulation>())
            cloneIndividualSimulationResults(sourceSimulation.DowncastTo<IndividualSimulation>(), targetSimulation.DowncastTo<IndividualSimulation>());
         else
            clonePopulationSimulationResults(sourceSimulation.DowncastTo<PopulationSimulation>(), targetSimulation.DowncastTo<PopulationSimulation>());

         //has to be done last
         targetSimulation.ResultsVersion = sourceSimulation.ResultsVersion;
      }

      public void CopyResults(Simulation sourceSimulation, Simulation targetSimulation)
      {
         if (!canCopyResults(sourceSimulation, targetSimulation)) return;

         if (targetSimulation.IsAnImplementationOf<IndividualSimulation>())
            copyResultsForIndividualSimulation(sourceSimulation.DowncastTo<IndividualSimulation>(), targetSimulation.DowncastTo<IndividualSimulation>());

         targetSimulation.ResultsVersion = sourceSimulation.ResultsVersion;
         sourceSimulation.Analyses.Each(targetSimulation.AddAnalysis);
      }

      private void clonePopulationSimulationResults(PopulationSimulation sourceSimulation, PopulationSimulation targetSimulation)
      {
         _simulationResultsSynchronizer.Synchronize(targetSimulation, targetSimulation.Results);
         sourceSimulation.Analyses.OfType<PopulationAnalysisChart>().Each(s => targetSimulation.AddAnalysis(_cloner.Clone(s)));
      }

      private void cloneIndividualSimulationResults(IndividualSimulation sourceSimulation, IndividualSimulation targetSimulation)
      {
         if (sourceSimulation.DataRepository.IsNull()) return;

         //Step 1 - update the results 
         targetSimulation.Results = _simulationResultsCreator.CreateResultsFrom(_cloner.Clone(sourceSimulation.DataRepository));

         //Step 2 - update the data repository based on the results created above
         targetSimulation.DataRepository = _dataRepositoryCreator.CreateResultsFor(targetSimulation);


         var curveNamesToRename = _curveNamer.CurvesWithOriginalName(sourceSimulation, sourceSimulation.Charts).Select(x => x.Name);

         sourceSimulation.SimulationCharts.Each(c => targetSimulation.AddAnalysis(_chartTemplatingTask.CloneChart(c, targetSimulation)));
         targetSimulation.Charts.SelectMany(chart => chart.Curves).Where(curve => curveNamesToRename.Contains(curve.Name)).
            Each(curve =>
            {
               curve.Name = _curveNamer.CurveNameForColumn(targetSimulation, curve.yData);
            });
      }

      private void copyResultsForIndividualSimulation(IndividualSimulation sourceSimulation, IndividualSimulation targetSimulation)
      {
         targetSimulation.Results = sourceSimulation.Results;
         targetSimulation.DataRepository = sourceSimulation.DataRepository;
      }

      private bool canCopyResults(Simulation sourceSimulation, Simulation targetSimulation)
      {
         //can only copy results if simulations have the same type 
         return targetSimulation.GetType() == sourceSimulation.GetType();
      }
   }
}