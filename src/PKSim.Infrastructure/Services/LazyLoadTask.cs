using System.Runtime.CompilerServices;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class LazyLoadTask : ILazyLoadTask
   {
      private readonly IContentLoader _contentLoader;
      private readonly ISimulationResultsLoader _simulationResultsLoader;
      private readonly ISimulationChartsLoader _simulationChartsLoader;
      private readonly IRegistrationTask _registrationTask;
      private readonly ISimulationComparisonContentLoader _simulationComparisonContentLoader;
      private readonly ISimulationAnalysesLoader _simulationAnalysesLoader;
      private readonly IParameterIdentificationContentLoader _parameterIdentificationContentLoader;
      private readonly ISensitivityAnalysisContentLoader _sensitivityAnalysisContentLoader;
      //each object is loaded under its own gate, taken on a private object rather than on the object's own monitor,
      //which any other code could take as well. The table is keyed by reference and holds its keys weakly, so a gate
      //never outlives the object it guards.
      //A load holds its gate while nested loads happen: deserializing a comparison, a parameter identification or a
      //chart loads the simulations it references. Those references only point from a container to a simulation and
      //never back, so gates are always taken in that one direction and cannot deadlock.
      //Completing a load also flags owned children as loaded without taking their gates (a simulation flags its
      //used building blocks, a population its base individual). That is safe because a child only becomes
      //reachable once its owner is loaded, and a load flags the children before publishing the owner's flag,
      //so a later Load on a child fast-paths out. The guarantee belongs to the load, not to the object graph:
      //BuildingBlockInSimulationSynchronizer can hand a loaded simulation an unloaded clone, which is then
      //loaded under its own gate like any other object.
      private static readonly ConditionalWeakTable<object, object> _loadGates = new ConditionalWeakTable<object, object>();

      public LazyLoadTask(
         IContentLoader contentLoader,
         ISimulationResultsLoader simulationResultsLoader,
         ISimulationChartsLoader simulationChartsLoader,
         ISimulationComparisonContentLoader simulationComparisonContentLoader,
         ISimulationAnalysesLoader simulationAnalysesLoader,
         IParameterIdentificationContentLoader parameterIdentificationContentLoader,
         ISensitivityAnalysisContentLoader sensitivityAnalysisContentLoader,
         IRegistrationTask registrationTask)
      {
         _contentLoader = contentLoader;
         _simulationResultsLoader = simulationResultsLoader;
         _simulationChartsLoader = simulationChartsLoader;
         _registrationTask = registrationTask;
         _simulationComparisonContentLoader = simulationComparisonContentLoader;
         _simulationAnalysesLoader = simulationAnalysesLoader;
         _parameterIdentificationContentLoader = parameterIdentificationContentLoader;
         _sensitivityAnalysisContentLoader = sensitivityAnalysisContentLoader;
      }

      private static object gateFor(object objectToLoad) => _loadGates.GetValue(objectToLoad, x => new object());

      //the gate is held for the whole load, so nothing reached from here may wait on the UI thread
      //(the closest edge is loadSimulations -> SimulationChartsLoader/ChartTask in PKSim.Presentation, clean today)
      public void Load<TObject>(TObject objectToLoad) where TObject : class, ILazyLoadable
      {
         //an object that is already loaded is the common case and must not pay for the gate
         if (objectToLoad == null || objectToLoad.IsLoaded) return;

         lock (gateFor(objectToLoad))
         {
            if (objectToLoad.IsLoaded) return;

            if (objectToLoad.IsAnImplementationOf<ISimulationComparison>())
               _simulationComparisonContentLoader.LoadContentFor(objectToLoad.DowncastTo<ISimulationComparison>());

            else if (objectToLoad.IsAnImplementationOf<ParameterIdentification>())
               _parameterIdentificationContentLoader.LoadContentFor(objectToLoad.DowncastTo<ParameterIdentification>());

            else if (objectToLoad.IsAnImplementationOf<SensitivityAnalysis>())
               _sensitivityAnalysisContentLoader.LoadContentFor(objectToLoad.DowncastTo<SensitivityAnalysis>());

            else if (objectToLoad.IsAnImplementationOf<IObjectBase>())
               loadObjectBase(objectToLoad as IObjectBase);

            else
               return;

            objectToLoad.IsLoaded = true;
         }
      }

      //no already-loaded fast path here: unlike IsLoaded, HasResults carries no publication guarantee, so the
      //gate is always taken - uncontended, it costs a few nanoseconds per call
      public void LoadResults<TSimulation>(TSimulation simulation) where TSimulation : Simulation
      {
         if (simulation == null)
            return;

         //one gate for both steps: loading the simulation may already have loaded its results
         lock (gateFor(simulation))
         {
            Load(simulation);

            if (simulation.HasResults)
               return;

            _simulationResultsLoader.LoadResultsFor(simulation);
         }
      }

      public void LoadResults(IPopulationDataCollector populationDataCollector)
      {
         var populationSimulationComparison = populationDataCollector as PopulationSimulationComparison;
         if (populationSimulationComparison == null)
            LoadResults(populationDataCollector as PopulationSimulation);
         else
            populationSimulationComparison.AllSimulations.Each(LoadResults);
      }

      private void loadObjectBase<T>(T objectToLoad) where T : IObjectBase
      {
         //first unregistered the object to load that might contain dummy objects that should be deleted
         _registrationTask.Unregister(objectToLoad);

         //load object content
         _contentLoader.LoadContentFor(objectToLoad);

         _registrationTask.Register(objectToLoad);

         //special loading steps for simulation
         loadSimulations(objectToLoad as Simulation);
      }

      private void loadSimulations(Simulation simulation)
      {
         if (simulation == null)
            return;

         //updating results may triggered update of has changed flag that is not accurate. We save the original state and update it at the end
         var hasChanged = simulation.HasChanged;

         //Only load results for individual simulations
         if (simulation.IsAnImplementationOf<IndividualSimulation>())
            _simulationResultsLoader.LoadResultsFor(simulation.DowncastTo<IndividualSimulation>());

         else if (simulation.IsAnImplementationOf<PopulationSimulation>())
            _simulationAnalysesLoader.LoadAnalysesFor(simulation.DowncastTo<PopulationSimulation>());

         //make sure each individual gets the expression profile defined in the simulation)  
         var simulationSubject = simulation.BuildingBlock<ISimulationSubject>();

         //this can happen for an imported simulation
         if (simulationSubject != null)
            simulation.AllBuildingBlocks<ExpressionProfile>().Each(simulationSubject.AddExpressionProfile);

         //in all cases, load the charts
         _simulationChartsLoader.LoadChartsFor(simulation);

         simulation.HasChanged = hasChanged;
      }
   }
}