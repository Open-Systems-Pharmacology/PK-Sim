using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using ModelPopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationComparisonMapper : ObjectBaseSnapshotMapperBase<ISimulationComparison, SimulationComparison>
   {
      private readonly IndividualSimulationComparisonMapper _individualSimulationComparisonMapper;
      private readonly PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;

      public SimulationComparisonMapper(IndividualSimulationComparisonMapper individualSimulationComparisonMapper, PopulationAnalysisChartMapper populationAnalysisChartMapper, IObjectBaseFactory objectBaseFactory)
      {
         _individualSimulationComparisonMapper = individualSimulationComparisonMapper;
         _populationAnalysisChartMapper = populationAnalysisChartMapper;
         _objectBaseFactory = objectBaseFactory;
      }

      public override async Task<SimulationComparison> MapToSnapshot(ISimulationComparison simulationComparison)
      {
         var snapshot = await SnapshotFrom(simulationComparison, x =>
            x.Simulations = simulationComparison.AllBaseSimulations.AllNames().ToArray()
         );

         await updateIndividualComparisonSnapshot(snapshot, simulationComparison);
         await updatePopulationComparisonSnapshot(snapshot, simulationComparison);
         return snapshot;
      }

      private async Task updatePopulationComparisonSnapshot(SimulationComparison snapshot, ISimulationComparison simulationComparison)
      {
         var populationSimulationComparison = simulationComparison as PopulationSimulationComparison;
         if (populationSimulationComparison == null)
            return;

         snapshot.PopulationComparisons = await _populationAnalysisChartMapper.MapToSnapshots(populationSimulationComparison.Analyses.OfType<ModelPopulationAnalysisChart>());
         snapshot.ReferenceGroupingItem = populationSimulationComparison.ReferenceGroupingItem;
         snapshot.ReferenceSimulation = populationSimulationComparison.ReferenceSimulation?.Name;
      }

      private async Task updateIndividualComparisonSnapshot(SimulationComparison snapshot, ISimulationComparison simulationComparison)
      {
         var individualSimulationComparison = simulationComparison as IndividualSimulationComparison;
         if (individualSimulationComparison == null)
            return;

         snapshot.IndividualComparison = await _individualSimulationComparisonMapper.MapToSnapshot(individualSimulationComparison);
      }

      public override async Task<ISimulationComparison> MapToModel(SimulationComparison snapshot, SnapshotContext snapshotContext)
      {
         var simulationComparison = await createSimulationComparisonFrom(snapshot, snapshotContext);
         MapSnapshotPropertiesToModel(snapshot, simulationComparison);
         simulationComparison.IsLoaded = true;
         return simulationComparison;
      }

      private async Task<ISimulationComparison> createSimulationComparisonFrom(SimulationComparison snapshot, SnapshotContext snapshotContext)
      {
         var project = snapshotContext.Project;
         if (snapshot.IndividualComparison != null)
         {
            var individualSimulationComparison = await createIndividualComparisonModel(snapshot, snapshotContext);
            return addSimulationsToComparison(individualSimulationComparison, snapshot, project);
         }

         var populationSimulationComparison = _objectBaseFactory.Create<PopulationSimulationComparison>();
         await updatePopulationComparisonModel(populationSimulationComparison, snapshot, snapshotContext);
         return addSimulationsToComparison(populationSimulationComparison, snapshot, project);
      }

      private async Task updatePopulationComparisonModel(PopulationSimulationComparison populationSimulationComparison, SimulationComparison snapshot, SnapshotContext snapshotContext)
      {
         var simulationComparisonContext = new SimulationAnalysisContext(snapshotContext.Project.AllObservedData, snapshotContext);
         var allPopulationAnalysis = await _populationAnalysisChartMapper.MapToModels(snapshot.PopulationComparisons, simulationComparisonContext);
         allPopulationAnalysis?.Each(populationSimulationComparison.AddAnalysis);
         populationSimulationComparison.ReferenceGroupingItem = snapshot.ReferenceGroupingItem;
         populationSimulationComparison.ReferenceSimulation = snapshotContext.Project.BuildingBlockByName<PopulationSimulation>(snapshot.ReferenceSimulation);
      }

      private Task<IndividualSimulationComparison> createIndividualComparisonModel(SimulationComparison snapshot, SnapshotContext snapshotContext)
      {
         var simulationComparisonContext = new SimulationAnalysisContext(snapshotContext.Project.AllObservedData, snapshotContext);
         var comparedSimulations = allComparedSimulationsFrom<IndividualSimulation>(snapshot, snapshotContext.Project);
         comparedSimulations.Each(s => simulationComparisonContext.AddDataRepository(s.DataRepository));
         return _individualSimulationComparisonMapper.MapToModel(snapshot.IndividualComparison, simulationComparisonContext);
      }

      private ISimulationComparison<TSimulation> addSimulationsToComparison<TSimulation>(ISimulationComparison<TSimulation> simulationComparison, SimulationComparison snapshot, PKSimProject project) where TSimulation : Model.Simulation
      {
         var allSimulations = allComparedSimulationsFrom<TSimulation>(snapshot, project);
         allSimulations.Each(simulationComparison.AddSimulation);
         return simulationComparison;
      }

      private IEnumerable<TSimulation> allComparedSimulationsFrom<TSimulation>(SimulationComparison snapshot, PKSimProject project) where TSimulation : Model.Simulation
      {
         if (snapshot.Simulations == null)
            return Enumerable.Empty<TSimulation>();

         return snapshot.Simulations.Select(project.BuildingBlockByName<TSimulation>).Where(s => s != null);
      }
   }
}