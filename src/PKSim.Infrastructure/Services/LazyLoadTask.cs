using PKSim.Assets;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;

namespace PKSim.Infrastructure.Services
{
   public class LazyLoadTask : ILazyLoadTask
   {
      private readonly IContentLoader _contentLoader;
      private readonly ISimulationResultsLoader _simulationResultsLoader;
      private readonly ISimulationChartsLoader _simulationChartsLoader;
      private readonly IRegistrationTask _registrationTask;
      private readonly IProgressManager _progressManager;
      private readonly ISimulationComparisonContentLoader _simulationComparisonContentLoader;
      private readonly ISimulationAnalysesLoader _simulationAnalysesLoader;
      private readonly IParameterIdentificationContentLoader _parameterIdentificationContentLoader;
      private readonly ISensitivityAnalysisContentLoader _sensitivityAnalysisContentLoader;

      public LazyLoadTask(IContentLoader contentLoader, ISimulationResultsLoader simulationResultsLoader, ISimulationChartsLoader simulationChartsLoader,
         ISimulationComparisonContentLoader simulationComparisonContentLoader, ISimulationAnalysesLoader simulationAnalysesLoader,
         IParameterIdentificationContentLoader parameterIdentificationContentLoader, ISensitivityAnalysisContentLoader sensitivityAnalysisContentLoader,
         IRegistrationTask registrationTask,
         IProgressManager progressManager)
      {
         _contentLoader = contentLoader;
         _simulationResultsLoader = simulationResultsLoader;
         _simulationChartsLoader = simulationChartsLoader;
         _registrationTask = registrationTask;
         _progressManager = progressManager;
         _simulationComparisonContentLoader = simulationComparisonContentLoader;
         _simulationAnalysesLoader = simulationAnalysesLoader;
         _parameterIdentificationContentLoader = parameterIdentificationContentLoader;
         _sensitivityAnalysisContentLoader = sensitivityAnalysisContentLoader;
      }

      public void Load<TObject>(TObject objectToLoad) where TObject : class, ILazyLoadable
      {
         if (objectToLoad == null || objectToLoad.IsLoaded) return;

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

      public void LoadResults<TSimulation>(TSimulation simulation) where TSimulation : Simulation
      {
         if (simulation == null || simulation.HasResults) return;
         _simulationResultsLoader.LoadResultsFor(simulation);
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
         using (var progressUpdater = _progressManager.Create())
         {
            progressUpdater.ReportStatusMessage(PKSimConstants.UI.LoadingObject(objectToLoad.Name));

            //first unregistered the object to load that might contain dummy objects that should be deleted
            _registrationTask.Unregister(objectToLoad);

            //load object content
            _contentLoader.LoadContentFor(objectToLoad);

            _registrationTask.Register(objectToLoad);

            //special loading steps for simulation
            loadSimulations(objectToLoad as Simulation);
         }
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

         //in all cases, load the charts
         _simulationChartsLoader.LoadChartsFor(simulation);

         simulation.HasChanged = hasChanged;
      }
   }
}