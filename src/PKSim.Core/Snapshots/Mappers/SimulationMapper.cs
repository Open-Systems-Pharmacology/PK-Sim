using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SnapshotSimulation = PKSim.Core.Snapshots.Simulation;
using ModelSimulation = PKSim.Core.Model.Simulation;
using ModelPopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationContext : SnapshotContext
   {
      public bool Run { get; }

      public SimulationContext(bool run, SnapshotContext baseContext) : base(baseContext)
      {
         Run = run;
      }

      public int NumberOfSimulationsToLoad { get; set; } = 1;
      public int NumberOfSimulationsLoaded { get; set; } = 1;
   }

   public class SimulationMapper : ObjectBaseSnapshotMapperBase<ModelSimulation, SnapshotSimulation, SimulationContext, PKSimProject>
   {
      private readonly SolverSettingsMapper _solverSettingsMapper;
      private readonly OutputSchemaMapper _outputSchemaMapper;
      private readonly OutputSelectionsMapper _outputSelectionsMapper;
      private readonly CompoundPropertiesMapper _compoundPropertiesMapper;
      private readonly ParameterMapper _parameterMapper;
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly EventMappingMapper _eventMappingMapper;
      private readonly ObserverSetMappingMapper _observerSetMappingMapper;
      private readonly SimulationTimeProfileChartMapper _simulationTimeProfileChartMapper;
      private readonly PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      private readonly ProcessMappingMapper _processMappingMapper;
      private readonly OutputMappingMapper _outputMappingMapper;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationModelCreator _simulationModelCreator;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly ISimulationRunner _simulationRunner;
      private readonly ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;
      private readonly IOSPSuiteLogger _logger;
      private readonly IContainerTask _containerTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IChartTask _chartTask;

      public SimulationMapper(
         SolverSettingsMapper solverSettingsMapper,
         OutputSchemaMapper outputSchemaMapper,
         OutputSelectionsMapper outputSelectionsMapper,
         CompoundPropertiesMapper compoundPropertiesMapper,
         ParameterMapper parameterMapper,
         AdvancedParameterMapper advancedParameterMapper,
         EventMappingMapper eventMappingMapper,
         ObserverSetMappingMapper observerSetMappingMapper,
         SimulationTimeProfileChartMapper simulationTimeProfileChartMapper,
         PopulationAnalysisChartMapper populationAnalysisChartMapper,
         ProcessMappingMapper processMappingMapper,
         OutputMappingMapper outputMappingMapper,
         ISimulationFactory simulationFactory,
         IExecutionContext executionContext,
         ISimulationModelCreator simulationModelCreator,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater,
         IModelPropertiesTask modelPropertiesTask,
         ISimulationRunner simulationRunner,
         ISimulationParameterOriginIdUpdater simulationParameterOriginIdUpdater,
         IOSPSuiteLogger logger,
         IContainerTask containerTask,
         IEntityPathResolver entityPathResolver,
         IChartTask chartTask)
      {
         _solverSettingsMapper = solverSettingsMapper;
         _outputSchemaMapper = outputSchemaMapper;
         _outputSelectionsMapper = outputSelectionsMapper;
         _compoundPropertiesMapper = compoundPropertiesMapper;
         _parameterMapper = parameterMapper;
         _advancedParameterMapper = advancedParameterMapper;
         _eventMappingMapper = eventMappingMapper;
         _observerSetMappingMapper = observerSetMappingMapper;
         _simulationTimeProfileChartMapper = simulationTimeProfileChartMapper;
         _populationAnalysisChartMapper = populationAnalysisChartMapper;
         _processMappingMapper = processMappingMapper;
         _outputMappingMapper = outputMappingMapper;
         _simulationFactory = simulationFactory;
         _executionContext = executionContext;
         _simulationModelCreator = simulationModelCreator;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _modelPropertiesTask = modelPropertiesTask;
         _simulationRunner = simulationRunner;
         _simulationParameterOriginIdUpdater = simulationParameterOriginIdUpdater;
         _logger = logger;
         _containerTask = containerTask;
         _entityPathResolver = entityPathResolver;
         _chartTask = chartTask;
      }

      public override async Task<SnapshotSimulation> MapToSnapshot(ModelSimulation simulation, PKSimProject project)
      {
         if (simulation.IsImported)
            throw new OSPSuiteException(PKSimConstants.Error.OnlyPKSimSimulationCanBeExportedToSnapshot(simulation.Name, simulation.Origin.DisplayName));

         var snapshot = await SnapshotFrom(simulation);
         snapshot.Individual = usedSimulationSubject<Model.Individual>(simulation, project);
         snapshot.Population = usedSimulationSubject<Model.Population>(simulation, project);
         snapshot.Compounds = await usedCompoundsFrom(simulation, project);
         snapshot.ObserverSets = await _observerSetMappingMapper.MapToSnapshots(simulation.ObserverSetProperties.ObserverSetMappings, project);
         snapshot.Model = simulation.ModelConfiguration.ModelName;
         snapshot.AllowAging = SnapshotValueFor(simulation.AllowAging, false);
         snapshot.Solver = await _solverSettingsMapper.MapToSnapshot(simulation.Solver);
         snapshot.OutputSchema = await _outputSchemaMapper.MapToSnapshot(simulation.OutputSchema);
         snapshot.OutputSelections = await _outputSelectionsMapper.MapToSnapshot(simulation.OutputSelections);
         snapshot.OutputMappings = await _outputMappingMapper.MapToSnapshots(simulation.OutputMappings.All);
         snapshot.Events = await _eventMappingMapper.MapToSnapshots(simulation.EventProperties.EventMappings, project);
         snapshot.ObservedData = usedObservedDataFrom(simulation, project);
         snapshot.Parameters = await allParametersChangedByUserFrom(simulation, project);
         snapshot.Interactions = await interactionSnapshotFrom(simulation.InteractionProperties);
         snapshot.AdvancedParameters = await advancedParametersFrom(simulation);
         snapshot.IndividualAnalyses = await _simulationTimeProfileChartMapper.MapToSnapshots(simulation.AnalysesOfType<SimulationTimeProfileChart>());
         snapshot.PopulationAnalyses = await _populationAnalysisChartMapper.MapToSnapshots(simulation.AnalysesOfType<ModelPopulationAnalysisChart>());
         snapshot.HasResults = simulation.HasResults;
         snapshot.AlteredBuildingBlocks = alteredBuildingBlocksIn(simulation);
         return snapshot;
      }

      private AlteredBuildingBlock[] alteredBuildingBlocksIn(ModelSimulation simulation)
      {
         var alteredUsedBuildingBlock = simulation.UsedBuildingBlocks.Where(x => x.Altered).ToList();
         if (!alteredUsedBuildingBlock.Any())
            return null;

         return alteredUsedBuildingBlock.Select(x => new AlteredBuildingBlock
         {
            Name = x.BuildingBlock.Name,
            Type = x.BuildingBlockType
         }).ToArray();
      }

      private Task<CompoundProcessSelection[]> interactionSnapshotFrom(InteractionProperties simulationInteractionProperties)
      {
         return _processMappingMapper.MapToSnapshots(simulationInteractionProperties.Interactions);
      }

      private Task<AdvancedParameter[]> advancedParametersFrom(ModelSimulation simulation)
      {
         var populationSimulation = simulation as PopulationSimulation;
         return _advancedParameterMapper.MapToSnapshots(populationSimulation?.AdvancedParameters);
      }

      private Task<CompoundProperties[]> usedCompoundsFrom(ModelSimulation simulation, PKSimProject project)
      {
         return _compoundPropertiesMapper.MapToSnapshots(simulation.CompoundPropertiesList, project);
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelSimulation simulation, PKSimProject project)
      {
         var allPotentialParametersToExport = simulation.Model.Root.GetAllChildren<IParameter>(p => p.ShouldExportToSnapshot()).GroupBy(x => x.BuildingBlockType);
         var allParametersToExport = new List<IParameter>();
         foreach (var parametersByBuildingBlockType in allPotentialParametersToExport)
         {
            var onlyChangedParameters = canFilterUnchangedParameters(parametersByBuildingBlockType.Key);
            allParametersToExport.AddRange(onlyChangedParameters ? parametersChangedFromBuildingBlock(parametersByBuildingBlockType, simulation, project) : parametersByBuildingBlockType);
         }

         return _parameterMapper.LocalizedParametersFrom(allParametersToExport);
      }

      private static bool canFilterUnchangedParameters(PKSimBuildingBlockType buildingBlockType)
      {
         //Exclude Protocol for now because of the 1to n relationship between building block parameters and simulation parameters
         return buildingBlockType.IsOneOf(
            PKSimBuildingBlockType.Compound,
            PKSimBuildingBlockType.Formulation,
            PKSimBuildingBlockType.Individual,
            PKSimBuildingBlockType.Event,
            PKSimBuildingBlockType.Population);
      }

      private IEnumerable<IParameter> parametersChangedFromBuildingBlock(IGrouping<PKSimBuildingBlockType, IParameter> parametersByBuildingBlockType, ModelSimulation simulation, PKSimProject project)
      {
         var parametersToExport = new List<IParameter>();
         var parametersByBuildingBlock = parametersByBuildingBlockType.GroupBy(x => x.Origin.BuilingBlockId);
         foreach (var parametersByBuildingBlockId in parametersByBuildingBlock)
         {
            var templateBuildingBlock = templateBuildingBlockFor(simulation, parametersByBuildingBlockId.Key, project);
            if (templateBuildingBlock == null)
            {
               parametersToExport.AddRange(parametersByBuildingBlockId);
               continue;
            }

            var templateBuildingBlockParameters = _containerTask.CacheAllChildren<IParameter>(templateBuildingBlock);

            foreach (var parameter in parametersByBuildingBlockId)
            {
               var templateBuildingBlockParameter = templateParameterFor(parameter, templateBuildingBlockParameters);

               if (parameterDiffersFromTemplate(templateBuildingBlockParameter, parameter))
               {
                  parametersToExport.Add(parameter);
               }
            }
         }

         return parametersToExport;
      }

      private static bool parameterDiffersFromTemplate(IParameter templateBuildingBlockParameter, IParameter parameter)
      {
         return templateBuildingBlockParameter == null ||
                !templateBuildingBlockParameter.ValueIsDefined() ||
                !ValueComparer.AreValuesEqual(parameter, templateBuildingBlockParameter);
      }

      private IParameter templateParameterFor(IParameter parameter, PathCache<IParameter> templateParameters)
      {
         var buildingBlockParameter = _executionContext.Get<IParameter>(parameter.Origin.ParameterId);
         if (buildingBlockParameter == null)
            return null;

         var buildingBlockParameterPath = _entityPathResolver.PathFor(buildingBlockParameter);
         return templateParameters[buildingBlockParameterPath];
      }

      private IPKSimBuildingBlock templateBuildingBlockFor(ModelSimulation simulation, string simulationBuildingBlockId, PKSimProject project)
      {
         var usedBuildingBlock = simulation.UsedBuildingBlockById(simulationBuildingBlockId);
         return usedBuildingBlock == null ? null : project.BuildingBlockById(usedBuildingBlock.TemplateId);
      }

      private string[] usedObservedDataFrom(ModelSimulation simulation, PKSimProject project)
      {
         if (!simulation.UsedObservedData.Any())
            return null;

         return simulation.UsedObservedData
            .Select(project.ObservedDataBy)
            .Select(x => x.Name).ToArray();
      }

      private string usedSimulationSubject<T>(ModelSimulation simulation, PKSimProject project) where T : class, ISimulationSubject
      {
         var usedBuildingBlock = simulation.UsedBuildingBlockInSimulation<T>();

         //simulation is not using this building block. 
         if (usedBuildingBlock == null)
            return null;

         var templateBuildingBlock = project.BuildingBlockById<T>(usedBuildingBlock.TemplateId);
         if (templateBuildingBlock == null)
            return null;

         if (typeof(T).IsAnImplementationOf<Model.Individual>() && simulation.IsAnImplementationOf<IndividualSimulation>())
            return templateBuildingBlock.Name;

         if (typeof(T).IsAnImplementationOf<Model.Population>() && simulation.IsAnImplementationOf<PopulationSimulation>())
            return templateBuildingBlock.Name;

         return null;
      }

      public override async Task<ModelSimulation> MapToModel(SnapshotSimulation snapshot, SimulationContext snapshotContext)
      {
         var project = snapshotContext.Project;
         _logger.AddInfo(PKSimConstants.Information.LoadingSimulation(snapshot.Name, snapshotContext.NumberOfSimulationsLoaded, snapshotContext.NumberOfSimulationsToLoad), project.Name);

         //Local cache of ids' that will be used to retrieve original building block parameters as the project is only registered 
         //in global context once the whole snapshot mapping process is completed

         var withIdRepository = new WithIdRepository();
         var registrationVisitor = new RegisterObjectVisitor(withIdRepository);

         var simulation = await createSimulationFrom(snapshot, snapshotContext);
         var contextWithSimulation = new SnapshotContextWithSimulation(simulation, snapshotContext);

         simulation.Solver = await _solverSettingsMapper.MapToModel(snapshot.Solver, snapshotContext);
         simulation.OutputSchema = await _outputSchemaMapper.MapToModel(snapshot.OutputSchema, snapshotContext);
         simulation.OutputSelections = await _outputSelectionsMapper.MapToModel(snapshot.OutputSelections, contextWithSimulation);

         var outputMappings = await _outputMappingMapper.MapToModels(snapshot.OutputMappings, contextWithSimulation);
         outputMappings?.Each(simulation.OutputMappings.Add);

         registrationVisitor.Register(simulation);

         await updateParameters(simulation, snapshot.Parameters, withIdRepository, snapshotContext);

         await updateAdvancedParameters(simulation, snapshot.AdvancedParameters, snapshotContext);

         updateUsedObservedData(simulation, snapshot.ObservedData, project);

         updateAlteredBuildingBlock(simulation, snapshot.AlteredBuildingBlocks);

         if (snapshotContext.Run)
            await runSimulation(snapshot, simulation);

         simulation.AddAnalyses(await individualAnalysesFrom(simulation, snapshot.IndividualAnalyses, snapshotContext));
         simulation.AddAnalyses(await populationAnalysesFrom(simulation, snapshot.PopulationAnalyses, snapshotContext));

         _simulationParameterOriginIdUpdater.UpdateSimulationId(simulation);
         _chartTask.UpdateObservedDataInChartsFor(simulation, snapshotContext.Project);
         return simulation;
      }

      private async Task updateInteractionProperties(ModelSimulation simulation, CompoundProcessSelection[] snapshotInteractions, SnapshotContext snapshotContext)
      {
         if (snapshotInteractions == null)
            return;

         var simulationSubject = simulation.BuildingBlock<ISimulationSubject>();
         foreach (var snapshotInteraction in snapshotInteractions)
         {
            var interaction = await interactionSelectionFrom(snapshotInteraction, simulationSubject, snapshotContext);
            if (interaction != null)
               simulation.InteractionProperties.AddInteraction(interaction);
         }
      }

      private async Task<InteractionSelection> interactionSelectionFrom(CompoundProcessSelection snapshotInteraction, ISimulationSubject simulationSubject, SnapshotContext snapshotContext)
      {
         var process = findProcess(snapshotContext.Project, snapshotInteraction, simulationSubject);
         if (process == null)
            return null;

         var processSelection = await _processMappingMapper.MapToModel(snapshotInteraction, new CompoundProcessSnapshotContext(process, snapshotContext));
         return processSelection.DowncastTo<InteractionSelection>();
      }

      private Model.CompoundProcess findProcess(PKSimProject project, CompoundProcessSelection snapshotInteraction, ISimulationSubject simulationSubject)
      {
         var compoundProcess = project.BuildingBlockByName<Model.Compound>(snapshotInteraction.CompoundName)
            ?.ProcessByName(snapshotInteraction.Name);

         if (compoundProcess != null)
            return compoundProcess;

         //No process found and a name was specified. This is a snapshot that is corrupted
         if (!string.IsNullOrEmpty(snapshotInteraction.Name))
         {
            _logger.AddWarning(PKSimConstants.Error.ProcessNotFoundInCompound(snapshotInteraction.Name, snapshotInteraction.CompoundName));
            return null;
         }

         //This might be a process that was deselected explicitly by the user
         var molecule = simulationSubject.MoleculeByName(snapshotInteraction.MoleculeName);
         return molecule == null ? null : new NoInteractionProcess {MoleculeName = molecule.Name};
      }

      private async Task runSimulation(SnapshotSimulation snapshot, ModelSimulation simulation)
      {
         if (!snapshot.HasResults)
            return;

         await _simulationRunner.RunSimulation(simulation);
      }

      private Task<SimulationTimeProfileChart[]> individualAnalysesFrom(ModelSimulation simulation, CurveChart[] snapshotCharts, SimulationContext simulationContext)
      {
         return analysesFrom(simulation, snapshotCharts, simulationContext, _simulationTimeProfileChartMapper.MapToModels);
      }

      private Task<ModelPopulationAnalysisChart[]> populationAnalysesFrom(ModelSimulation simulation, PopulationAnalysisChart[] snapshotPopulationAnalyses, SimulationContext simulationContext)
      {
         return analysesFrom(simulation, snapshotPopulationAnalyses, simulationContext, _populationAnalysisChartMapper.MapToModels);
      }

      private Task<TAnalysis[]> analysesFrom<TAnalysis, TSnapshotAnalysis>(ModelSimulation simulation, TSnapshotAnalysis[] snapshotAnalyses, SimulationContext simulationContext,
         Func<TSnapshotAnalysis[], SimulationAnalysisContext, Task<TAnalysis[]>> mapFunc)
      {
         if (snapshotAnalyses == null)
            return Task.FromResult(new List<TAnalysis>().ToArray());

         var project = simulationContext.Project;
         var curveChartContext = new SimulationAnalysisContext(project.AllObservedData, simulationContext) {RunSimulation = simulationContext.Run};

         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation?.DataRepository != null)
            curveChartContext.AddDataRepository(individualSimulation.DataRepository);

         return mapFunc(snapshotAnalyses, curveChartContext);
      }

      private async Task<ModelSimulation> createSimulationFrom(SnapshotSimulation snapshot, SnapshotContext snapshotContext)
      {
         var simulation = await createModelLessSimulationFrom(snapshot, snapshotContext);
         _simulationModelCreator.CreateModelFor(simulation);
         return simulation;
      }

      private async Task<ModelSimulation> createModelLessSimulationFrom(SnapshotSimulation snapshot, SnapshotContext snapshotContext)
      {
         var project = snapshotContext.Project;
         var simulationSubject = simulationSubjectFrom(snapshot, project);
         var compounds = compoundsFrom(snapshot.Compounds, project);
         var modelProperties = modelPropertiesFrom(snapshot.Model, simulationSubject);

         var simulation = _simulationFactory.CreateFrom(simulationSubject, compounds, modelProperties);
         MapSnapshotPropertiesToModel(snapshot, simulation);

         await mapCompoundProperties(simulation, snapshot.Compounds, snapshotContext);
         simulation.EventProperties = await mapEventProperties(snapshot.Events, snapshotContext);
         simulation.ObserverSetProperties = await mapObserverSetProperties(snapshot.ObserverSets, snapshotContext);
         await updateInteractionProperties(simulation, snapshot.Interactions, snapshotContext);

         //Once all used building blocks have been set, we need to ensure that they are also synchronized in the  simulation
         updateUsedBuildingBlockInSimulation(simulation, project);
         return simulation;
      }

      private async Task<EventProperties> mapEventProperties(EventSelection[] snapshotEvents, SnapshotContext snapshotContext)
      {
         var eventProperties = new EventProperties();
         var events = await _eventMappingMapper.MapToModels(snapshotEvents, snapshotContext);
         events?.Each(eventProperties.AddEventMapping);
         return eventProperties;
      }

      private async Task<ObserverSetProperties> mapObserverSetProperties(ObserverSetSelection[] snapshotObserverSet, SnapshotContext snapshotContext)
      {
         var observerSetProperties = new ObserverSetProperties();
         var observerSetMappings = await _observerSetMappingMapper.MapToModels(snapshotObserverSet, snapshotContext);
         observerSetMappings?.Each(observerSetProperties.AddObserverSetMapping);
         return observerSetProperties;
      }

      private void updateUsedBuildingBlockInSimulation(ModelSimulation simulation, PKSimProject project)
      {
         _simulationBuildingBlockUpdater.UpdateFormulationsInSimulation(simulation);
         _simulationBuildingBlockUpdater.UpdateProtocolsInSimulation(simulation);

         var events = simulation.EventProperties.EventMappings.Select(x => project.BuildingBlockById<PKSimEvent>(x.TemplateEventId));
         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, events, PKSimBuildingBlockType.Event);

         var observerSets = simulation.ObserverSetProperties.ObserverSetMappings.Select(x => project.BuildingBlockById<Model.ObserverSet>(x.TemplateObserverSetId));
         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, observerSets, PKSimBuildingBlockType.ObserverSet);
      }

      private Task mapCompoundProperties(ModelSimulation simulation, CompoundProperties[] snapshotCompoundProperties, SnapshotContext snapshotContext)
      {
         var compoundPropertiesContext = new SnapshotContextWithSimulation(simulation, snapshotContext);
         return _compoundPropertiesMapper.MapToModels(snapshotCompoundProperties, compoundPropertiesContext);
      }

      private Task updateAdvancedParameters(ModelSimulation simulation, AdvancedParameter[] snapshotAdvancedParameters, SnapshotContext snapshotContext)
      {
         return _advancedParameterMapper.MapToModel(snapshotAdvancedParameters, simulation as PopulationSimulation, snapshotContext);
      }

      private async Task updateParameters(ModelSimulation simulation, LocalizedParameter[] snapshotParameters, WithIdRepository withIdRepository, SnapshotContext snapshotContext)
      {
         await _parameterMapper.MapLocalizedParameters(snapshotParameters, simulation.Model.Root, snapshotContext);

         //The parameter mapper does not update the original building block in the simulation. We need to make sure we are keeping these values in sync so 
         // that update and commit will work as expected (or building block difference )
         var allParameters = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         snapshotParameters?.Each(x =>
         {
            var parameter = allParameters[x.Path];
            //Somehow the localized parameter path was edited by hand or does not exist anymore. Warning was already shown during mapping. 
            if (parameter == null)
               return;

            var buildingBlockParameter = withIdRepository.Get<IParameter>(parameter.Origin.ParameterId);
            if (buildingBlockParameter == null || !parameterDiffersFromTemplate(buildingBlockParameter, parameter))
               return;

            buildingBlockParameter.Value = parameter.Value;
            buildingBlockParameter.IsDefault = false;
         });
      }

      private void updateUsedObservedData(ModelSimulation simulation, string[] snapshotObservedData, PKSimProject project)
      {
         snapshotObservedData?.Each(observedDataName =>
         {
            var observedData = project.AllObservedData.FindByName(observedDataName);
            simulation.AddUsedObservedData(observedData);
         });
      }

      private void updateAlteredBuildingBlock(ModelSimulation simulation, AlteredBuildingBlock[] snapshotAlteredBuildingBlocks)
      {
         snapshotAlteredBuildingBlocks?.Each(x =>
         {
            //At this point, used building block and building block have the same name by construction
            var usedBuildingBlock = simulation.UsedBuildingBlocksInSimulation(x.Type).Find(bb => bb.IsNamed(x.Name));
            if (usedBuildingBlock == null)
            {
               _logger.AddWarning(PKSimConstants.Error.AlteredBuildingBlockNotFoundInSimulation(simulation.Name, x.Name, x.Type.ToString()));
               return;
            }

            usedBuildingBlock.Altered = true;
            usedBuildingBlock.Name = CoreConstants.ContainerName.BuildingBlockInSimulationNameFor(x.Name, simulation.Name);
         });
      }

      protected override void MapSnapshotPropertiesToModel(SnapshotSimulation snapshot, ModelSimulation simulation)
      {
         base.MapSnapshotPropertiesToModel(snapshot, simulation);
         simulation.AllowAging = ModelValueFor(snapshot.AllowAging, false);
      }

      private ModelProperties modelPropertiesFrom(string modelName, ISimulationSubject simulationSubject)
      {
         return _modelPropertiesTask.DefaultFor(simulationSubject.OriginData, modelName);
      }

      private IReadOnlyList<Model.Compound> compoundsFrom(CompoundProperties[] compounds, PKSimProject project)
      {
         return compounds.Select(x => findBuildingBlock<Model.Compound>(x.Name, project)).ToList();
      }

      private ISimulationSubject simulationSubjectFrom(SnapshotSimulation snapshot, PKSimProject project)
      {
         if (snapshot.Individual != null)
            return findBuildingBlock<Model.Individual>(snapshot.Individual, project);

         if (snapshot.Population != null)
            return findBuildingBlock<Model.Population>(snapshot.Population, project);

         throw new SnapshotOutdatedException(PKSimConstants.Error.SimulationSubjectUndefinedInSnapshot);
      }

      private T findBuildingBlock<T>(string name, PKSimProject project) where T : class, IPKSimBuildingBlock
      {
         var buildingBlock = project.BuildingBlockByName<T>(name);
         if (buildingBlock == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.SimulationTemplateBuildingBlockNotFoundInProject(name, typeof(T).Name));

         _executionContext.Load(buildingBlock);
         return buildingBlock;
      }
   }
}