using System;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core.Services
{
   public interface IImportSimulationTask
   {
      PopulationSimulationImport ImportFromBuidlingBlock(string simulationFile, Population population);
      PopulationSimulationImport ImportFromPopulationFile(string simulationFile, string populationFile);
      PopulationSimulationImport ImportFromPopulationSize(string simulationFile, int numberOfIndividuals);
      IndividualSimulation ImportIndividualSimulation(string pkmlFileFullPath);
   }

   public class ImportSimulationTask : IImportSimulationTask
   {
      private readonly ISimulationTransferLoader _simulationTransferLoader;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IEntitiesInContainerRetriever _parameterRetriever;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IIndividualPropertiesCacheImporter _individualPropertiesCacheImporter;
      private readonly IExecutionContext _executionContext;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISimulationUpdaterAfterDeserialization _simulationUpdaterAfterDeserialization;
      private readonly IAdvancedParameterFactory _advancedParameterFactory;

      public ImportSimulationTask(ISimulationTransferLoader simulationTransferLoader, ISimulationFactory simulationFactory, IEntitiesInContainerRetriever parameterRetriever, ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater,
         IIndividualPropertiesCacheImporter individualPropertiesCacheImporter, IExecutionContext executionContext, IObjectBaseFactory objectBaseFactory, 
         ISimulationUpdaterAfterDeserialization simulationUpdaterAfterDeserialization, IAdvancedParameterFactory advancedParameterFactory)
      {
         _simulationTransferLoader = simulationTransferLoader;
         _simulationFactory = simulationFactory;
         _parameterRetriever = parameterRetriever;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _individualPropertiesCacheImporter = individualPropertiesCacheImporter;
         _executionContext = executionContext;
         _objectBaseFactory = objectBaseFactory;
         _simulationUpdaterAfterDeserialization = simulationUpdaterAfterDeserialization;
         _advancedParameterFactory = advancedParameterFactory;
      }

      public PopulationSimulationImport ImportFromBuidlingBlock(string simulationFile, Population population)
      {
         _executionContext.Load(population);
         var populationSimulationImport = populationSimulationImportFrom(simulationFile);
         return addPopulationToPopulationSimulation(populationSimulationImport, population);
      }

      public PopulationSimulationImport ImportFromPopulationFile(string simulationFile, string populationFile)
      {
         var populationSimulationImport = populationSimulationImportFrom(simulationFile);
         var cache = _individualPropertiesCacheImporter.ImportFrom(populationFile, populationSimulationImport);
         if (populationSimulationImport.Status.Is(NotificationType.Error))
            return populationSimulationImport;

         var population = _objectBaseFactory.Create<MoBiPopulation>();
         population.SetNumberOfItems(cache.Count);

         var populationSimulation = populationSimulationImport.PopulationSimulation;
         var allParameters = _parameterRetriever.ParametersFrom(populationSimulation);

         foreach (var parameterPath in cache.AllParameterPaths())
         {
            var parameter = allParameters[parameterPath];
            if (parameter != null)
            {
               populationSimulation.ParameterValuesCache.Add(cache.ParameterValuesFor(parameterPath));
               addAdvancedParameterIfRequired(parameter, populationSimulation);
            }
            else
               populationSimulationImport.AddWarning(PKSimConstants.Warning.ParameterPathNotFoundInSimulationAndWillBeIgnored(parameterPath));
         }

         return addPopulationToPopulationSimulation(populationSimulationImport, population);
      }

      private void addAdvancedParameterIfRequired(IParameter parameter, PopulationSimulation populationSimulation)
      {
         if (!parameter.CanBeDefinedAsAdvanced())
            return;
         
         var advancedParameter = _advancedParameterFactory.Create(parameter, DistributionTypes.Unknown);

         //do not generate random values as these were loaded from files
         populationSimulation.AddAdvancedParameter(advancedParameter, generateRandomValues: false);
      }

      public PopulationSimulationImport ImportFromPopulationSize(string simulationFile, int numberOfIndividuals)
      {
         var populationSimulationImport = populationSimulationImportFrom(simulationFile);
         var population = _objectBaseFactory.Create<MoBiPopulation>();
         var advancedParameter = _objectBaseFactory.Create<AdvancedParameter>();
         population.SetNumberOfItems(numberOfIndividuals);

         var populationSimulation = populationSimulationImport.PopulationSimulation;
         foreach (var parameter in _parameterRetriever.ParametersFrom(populationSimulationImport.PopulationSimulation, x => x.IsDistributed()).KeyValues)
         {
            //we need to use a clone to avoid resetting parent relationship
            advancedParameter.DistributedParameter = _executionContext.Clone(parameter.Value.DowncastTo<IDistributedParameter>());
            populationSimulation.ParameterValuesCache.SetValues(parameter.Key, advancedParameter.GenerateRandomValues(numberOfIndividuals));
         }

         return addPopulationToPopulationSimulation(populationSimulationImport, population);
      }

      private PopulationSimulationImport addPopulationToPopulationSimulation(PopulationSimulationImport populationSimulationImport, Population population)
      {
         addPopulationBuildingBlockToSimulation(populationSimulationImport, population);
         addInfoToLogForSuccessfulImport(populationSimulationImport);
         return populationSimulationImport;
      }

      public IndividualSimulation ImportIndividualSimulation(string pkmlFileFullPath)
      {
         return createSimulationFor<IndividualSimulation>(pkmlFileFullPath);
      }

      private TSimulation createSimulationFor<TSimulation>(string pkmlFileFullPath) where TSimulation : Simulation
      {
         try
         {
            var loadedSim = _simulationTransferLoader.Load(pkmlFileFullPath);
            if (loadedSim == null)
               return null;

            var simulation = _simulationFactory.CreateBasedOn<TSimulation>(loadedSim.Simulation);
            _simulationUpdaterAfterDeserialization.UpdateSimulation(simulation);
            return simulation;
         }
         catch (Exception e)
         {
            throw new PKSimException(PKSimConstants.Error.CouldNotLoadSimulationFromFile(pkmlFileFullPath), e);
         }
      }

      private void addInfoToLogForSuccessfulImport(PopulationSimulationImport import)
      {
         if (import.Status.Is(NotificationType.Error))
            return;

         var populationSimulation = import.PopulationSimulation;
         import.AddInfo(PKSimConstants.Information.PopulationSimulationSuccessfullyImported(populationSimulation.Name, populationSimulation.NumberOfItems));
      }

      private void addPopulationBuildingBlockToSimulation(PopulationSimulationImport populationSimulationImport, Population population)
      {
         _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(populationSimulationImport.PopulationSimulation, population, PKSimBuildingBlockType.SimulationSubject);
      }

      private PopulationSimulationImport populationSimulationImportFrom(string pkmlFileFullPath)
      {
         var populationSimulation = createSimulationFor<PopulationSimulation>(pkmlFileFullPath);
         return new PopulationSimulationImport {PopulationSimulation = populationSimulation};
      }
   }
}