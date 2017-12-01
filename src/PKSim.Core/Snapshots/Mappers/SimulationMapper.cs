using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
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
   public class SimulationMapper : ObjectBaseSnapshotMapperBase<ModelSimulation, SnapshotSimulation, PKSimProject, PKSimProject>
   {
      private readonly SolverSettingsMapper _solverSettingsMapper;
      private readonly OutputSchemaMapper _outputSchemaMapper;
      private readonly OutputSelectionsMapper _outputSelectionsMapper;
      private readonly CompoundPropertiesMapper _compoundPropertiesMapper;
      private readonly ParameterMapper _parameterMapper;
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly EventMappingMapper _eventMappingMapper;
      private readonly SimulationTimeProfileChartMapper _simulationTimeProfileChartMapper;
      private readonly PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      private readonly ProcessMappingMapper _processMappingMapper;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationModelCreator _simulationModelCreator;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly ISimulationRunner _simulationRunner;
      private readonly ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;

      public SimulationMapper(
         SolverSettingsMapper solverSettingsMapper,
         OutputSchemaMapper outputSchemaMapper,
         OutputSelectionsMapper outputSelectionsMapper,
         CompoundPropertiesMapper compoundPropertiesMapper,
         ParameterMapper parameterMapper,
         AdvancedParameterMapper advancedParameterMapper,
         EventMappingMapper eventMappingMapper,
         SimulationTimeProfileChartMapper simulationTimeProfileChartMapper,
         PopulationAnalysisChartMapper populationAnalysisChartMapper,
         ProcessMappingMapper processMappingMapper,
         ISimulationFactory simulationFactory,
         IExecutionContext executionContext,
         ISimulationModelCreator simulationModelCreator,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater,
         IModelPropertiesTask modelPropertiesTask,
         ISimulationRunner simulationRunner,
         ISimulationParameterOriginIdUpdater simulationParameterOriginIdUpdater
      )
      {
         _solverSettingsMapper = solverSettingsMapper;
         _outputSchemaMapper = outputSchemaMapper;
         _outputSelectionsMapper = outputSelectionsMapper;
         _compoundPropertiesMapper = compoundPropertiesMapper;
         _parameterMapper = parameterMapper;
         _advancedParameterMapper = advancedParameterMapper;
         _eventMappingMapper = eventMappingMapper;
         _simulationTimeProfileChartMapper = simulationTimeProfileChartMapper;
         _populationAnalysisChartMapper = populationAnalysisChartMapper;
         _processMappingMapper = processMappingMapper;
         _simulationFactory = simulationFactory;
         _executionContext = executionContext;
         _simulationModelCreator = simulationModelCreator;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _modelPropertiesTask = modelPropertiesTask;
         _simulationRunner = simulationRunner;
         _simulationParameterOriginIdUpdater = simulationParameterOriginIdUpdater;
      }

      public override async Task<SnapshotSimulation> MapToSnapshot(ModelSimulation simulation, PKSimProject project)
      {
         if (simulation.Origin != Origins.PKSim)
            throw new OSPSuiteException(PKSimConstants.Error.OnlyPKSimSimulationCanBeExportedToSnapshot(simulation.Name, simulation.Origin.DisplayName));

         var snapshot = await SnapshotFrom(simulation);
         snapshot.Individual = usedSimulationSubject<Model.Individual>(simulation);
         snapshot.Population = usedSimulationSubject<Model.Population>(simulation);
         snapshot.Compounds = await usedCompoundsFrom(simulation, project);
         snapshot.Model = simulation.ModelConfiguration.ModelName;
         snapshot.AllowAging = SnapshotValueFor(simulation.AllowAging, false);
         snapshot.Solver = await _solverSettingsMapper.MapToSnapshot(simulation.Solver);
         snapshot.OutputSchema = await _outputSchemaMapper.MapToSnapshot(simulation.OutputSchema);
         snapshot.OutputSelections = await _outputSelectionsMapper.MapToSnapshot(simulation.OutputSelections);
         snapshot.Events = await _eventMappingMapper.MapToSnapshots(simulation.EventProperties.EventMappings, project);
         snapshot.ObservedData = usedObervedDataFrom(simulation, project);
         snapshot.Parameters = await allParametersChangedByUserFrom(simulation);
         snapshot.Interactions = await interactionSnapshotFrom(simulation.InteractionProperties);
         snapshot.AdvancedParameters = await advancedParametersFrom(simulation);
         snapshot.IndividualAnalyses = await _simulationTimeProfileChartMapper.MapToSnapshots(simulation.AnalysesOfType<SimulationTimeProfileChart>());
         snapshot.PopulationAnalyses = await _populationAnalysisChartMapper.MapToSnapshots(simulation.AnalysesOfType<ModelPopulationAnalysisChart>());
         snapshot.HasResults = simulation.HasResults;
         return snapshot;
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

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelSimulation simulation)
      {
         var changedParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.ParameterHasChanged());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      private string[] usedObervedDataFrom(ModelSimulation simulation, PKSimProject project)
      {
         if (!simulation.UsedObservedData.Any())
            return null;

         return simulation.UsedObservedData
            .Select(project.ObservedDataBy)
            .Select(x => x.Name).ToArray();
      }

      private string usedSimulationSubject<T>(ModelSimulation simulation) where T : class, ISimulationSubject
      {
         var name = simulation.BuildingBlock<T>()?.Name;
         if (typeof(T).IsAnImplementationOf<Model.Individual>() && simulation.IsAnImplementationOf<IndividualSimulation>())
            return name;

         if (typeof(T).IsAnImplementationOf<Model.Population>() && simulation.IsAnImplementationOf<PopulationSimulation>())
            return name;

         return null;
      }

      public override async Task<ModelSimulation> MapToModel(SnapshotSimulation snapshot, PKSimProject project)
      {
         var simulation = await createSimulationFrom(snapshot, project);

         simulation.Solver = await _solverSettingsMapper.MapToModel(snapshot.Solver);
         simulation.OutputSchema = await _outputSchemaMapper.MapToModel(snapshot.OutputSchema);
         simulation.OutputSelections = await _outputSelectionsMapper.MapToModel(snapshot.OutputSelections, simulation);

         await updateParameters(simulation, snapshot.Parameters);
         await updateAdvancedParameters(simulation, snapshot.AdvancedParameters);

         updateUsedObservedData(simulation, snapshot.ObservedData, project);

         await runSimulation(snapshot, simulation);

         simulation.AddAnalyses(await individualAnalysesFrom(simulation, snapshot.IndividualAnalyses, project));
         simulation.AddAnalyses(await populationAnalysesFrom(simulation, snapshot.PopulationAnalyses, project));

         _simulationParameterOriginIdUpdater.UpdateSimulationId(simulation);
         return simulation;
      }

      private async Task updateInteractonProperties(ModelSimulation simulation, CompoundProcessSelection[] snapshotInteractions, PKSimProject project)
      {
         if (snapshotInteractions == null)
            return;

         foreach (var snapshotInteraction in snapshotInteractions)
         {
            var interaction = await interactionSelectionFrom(snapshotInteraction, project);
            simulation.InteractionProperties.AddInteraction(interaction);
         }
      }

      private async Task<InteractionSelection> interactionSelectionFrom(CompoundProcessSelection snapshotInteraction, PKSimProject project)
      {
         var process = findProcess(project, snapshotInteraction);
         if (process == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.ProcessNotFoundInCompound(snapshotInteraction.Name, snapshotInteraction.CompoundName));

         var processSelection = await _processMappingMapper.MapToModel(snapshotInteraction, process);
         return processSelection.DowncastTo<InteractionSelection>();
      }

      private Model.CompoundProcess findProcess(PKSimProject project, CompoundProcessSelection snapshotInteraction)
      {
         return project.BuildingBlockByName<Model.Compound>(snapshotInteraction.CompoundName)
            ?.ProcessByName(snapshotInteraction.Name);
      }

      private async Task runSimulation(SnapshotSimulation snapshot, ModelSimulation simulation)
      {
         if (!snapshot.HasResults)
            return;

         await _simulationRunner.RunSimulation(simulation);
      }

      private Task<SimulationTimeProfileChart[]> individualAnalysesFrom(ModelSimulation simulation, CurveChart[] snapshotCharts, PKSimProject project)
      {
         return analysesFrom(simulation, snapshotCharts, project, _simulationTimeProfileChartMapper.MapToModels);
      }

      private Task<ModelPopulationAnalysisChart[]> populationAnalysesFrom(ModelSimulation simulation, PopulationAnalysisChart[] snapshotPopulationAnalyses, PKSimProject project)
      {
         return analysesFrom(simulation, snapshotPopulationAnalyses, project, _populationAnalysisChartMapper.MapToModels);
      }

      private Task<TAnalysis[]> analysesFrom<TAnalysis, TSnapshotAnalysis>(ModelSimulation simulation, TSnapshotAnalysis[] snapshotAnalyses, PKSimProject project,
         Func<TSnapshotAnalysis[], SimulationAnalysisContext, Task<TAnalysis[]>> mapFunc)
      {
         if (snapshotAnalyses == null)
            return Task.FromResult(new List<TAnalysis>().ToArray());

         var curveChartContext = new SimulationAnalysisContext(project.AllObservedData);

         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation?.DataRepository != null)
            curveChartContext.AddDataRepository(individualSimulation.DataRepository);

         return mapFunc(snapshotAnalyses, curveChartContext);
      }

      private async Task<ModelSimulation> createSimulationFrom(SnapshotSimulation snapshot, PKSimProject project)
      {
         var simulation = await createModelLessSimulationFrom(snapshot, project);
         _simulationModelCreator.CreateModelFor(simulation);
         return simulation;
      }

      private async Task<ModelSimulation> createModelLessSimulationFrom(SnapshotSimulation snapshot, PKSimProject project)
      {
         var simulationSubject = simulationSubjectFrom(snapshot, project);
         var compounds = compoundsFrom(snapshot.Compounds, project);
         var modelProperties = modelPropertiesFrom(snapshot.Model, simulationSubject);

         var simulation = _simulationFactory.CreateFrom(simulationSubject, compounds, modelProperties);
         MapSnapshotPropertiesToModel(snapshot, simulation);

         await mapCompoundProperties(simulation, snapshot.Compounds, project);
         simulation.EventProperties = await mapEventProperties(snapshot.Events, project);
         await updateInteractonProperties(simulation, snapshot.Interactions, project);

         //Once all used building blocks have been set, we need to ensure that they are also synchronized in the  simulation
         updateUsedBuildingBlockInSimulation(simulation, project);
         return simulation;
      }

      private async Task<EventProperties> mapEventProperties(EventSelection[] snapshotEvents, PKSimProject project)
      {
         var eventProperties = new EventProperties();
         var events = await _eventMappingMapper.MapToModels(snapshotEvents, project);
         events?.Each(eventProperties.AddEventMapping);
         return eventProperties;
      }

      private void updateUsedBuildingBlockInSimulation(ModelSimulation simulation, PKSimProject project)
      {
         _simulationBuildingBlockUpdater.UpdateFormulationsInSimulation(simulation);
         _simulationBuildingBlockUpdater.UpdateProtocolsInSimulation(simulation);

         var events = simulation.EventProperties.EventMappings.Select(x => project.BuildingBlockById<PKSimEvent>(x.TemplateEventId));
         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, events, PKSimBuildingBlockType.Event);
      }

      private Task mapCompoundProperties(ModelSimulation simulation, CompoundProperties[] snapshotCompoundProperties, PKSimProject project)
      {
         var compoundPropertiesContext = new CompoundPropertiesContext(project, simulation);
         return _compoundPropertiesMapper.MapToModels(snapshotCompoundProperties, compoundPropertiesContext);
      }

      private Task updateAdvancedParameters(ModelSimulation simulation, AdvancedParameter[] snapshotAdvancedParameters)
      {
         return _advancedParameterMapper.MapToModel(snapshotAdvancedParameters, simulation as PopulationSimulation);
      }

      private Task updateParameters(ModelSimulation simulation, LocalizedParameter[] snapshotParameters)
      {
         return _parameterMapper.MapLocalizedParameters(snapshotParameters, simulation.Model.Root);
      }

      private void updateUsedObservedData(ModelSimulation simulation, string[] snapshotObservedData, PKSimProject project)
      {
         snapshotObservedData?.Each(observedDataName =>
         {
            var observedData = project.AllObservedData.FindByName(observedDataName);
            simulation.AddUsedObservedData(observedData);
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
            throw new SnapshotOutdatedException(PKSimConstants.Error.SimulationTemplateBuildingBlocktNotFoundInProject(name, typeof(T).Name));

         _executionContext.Load(buildingBlock);
         return buildingBlock;
      }
   }
}