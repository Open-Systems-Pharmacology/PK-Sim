using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ModelParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;
using ModelProject = PKSim.Core.Model.PKSimProject;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelSimulation = PKSim.Core.Model.Simulation;
using ModelPopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ProjectMapper<ModelProject, SnapshotProject, ProjectContext>
   {
      private readonly SimulationMapper _simulationMapper;
      private readonly SimulationComparisonMapper _simulationComparisonMapper;
      private readonly QualificationPlanMapper _qualificationPlanMapper;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly ICoreUserSettings _userSettings;
      private readonly ISimulationRunner _simulationRunner;
      private readonly SimulationTimeProfileChartMapper _simulationTimeProfileChartMapper;
      private readonly PopulationAnalysisChartMapper _populationAnalysisChartMapper;

      public ProjectMapper(
         SimulationMapper simulationMapper,
         SimulationComparisonMapper simulationComparisonMapper,
         ParameterIdentificationMapper parameterIdentificationMapper,
         QualificationPlanMapper qualificationPlanMapper,
         IExecutionContext executionContext,
         IClassificationSnapshotTask classificationSnapshotTask,
         ILazyLoadTask lazyLoadTask,
         ICreationMetaDataFactory creationMetaDataFactory,
         IOSPSuiteLogger logger,
         ICoreUserSettings userSettings,
         ISimulationRunner simulationRunner,
         SimulationTimeProfileChartMapper simulationTimeProfileChartMapper,
         PopulationAnalysisChartMapper populationAnalysisChartMapper
      ) : base(creationMetaDataFactory, logger, executionContext, classificationSnapshotTask, parameterIdentificationMapper)
      {
         _simulationMapper = simulationMapper;
         _simulationComparisonMapper = simulationComparisonMapper;
         _qualificationPlanMapper = qualificationPlanMapper;
         _lazyLoadTask = lazyLoadTask;
         _userSettings = userSettings;
         _simulationRunner = simulationRunner;
         _simulationTimeProfileChartMapper = simulationTimeProfileChartMapper;
         _populationAnalysisChartMapper = populationAnalysisChartMapper;
      }

      public override async Task<SnapshotProject> MapToSnapshot(ModelProject project)
      {
         var snapshot = await SnapshotFrom(project, x =>
         {
            x.Version = ProjectVersions.Current;
            x.Name = SnapshotValueFor(project.Name);
            x.Description = SnapshotValueFor(project.Description);
         });

         //first pass. We load all building blocks to ensure that there are no issues with the building blocks loading order
         project.All<IPKSimBuildingBlock>().Each(_lazyLoadTask.Load);

         snapshot.ExpressionProfiles = await mapBuildingBlocksToSnapshots<ExpressionProfile>(project.All<Model.ExpressionProfile>());
         snapshot.Individuals = await mapBuildingBlocksToSnapshots<Individual>(project.All<Model.Individual>());
         snapshot.Compounds = await mapBuildingBlocksToSnapshots<Compound>(project.All<Model.Compound>());
         snapshot.Events = await mapBuildingBlocksToSnapshots<Event>(project.All<PKSimEvent>());
         snapshot.Formulations = await mapBuildingBlocksToSnapshots<Formulation>(project.All<Model.Formulation>());
         snapshot.Protocols = await mapBuildingBlocksToSnapshots<Protocol>(project.All<Model.Protocol>());
         snapshot.Populations = await mapBuildingBlocksToSnapshots<Population>(project.All<Model.Population>());
         snapshot.ObserverSets = await mapBuildingBlocksToSnapshots<ObserverSet>(project.All<Model.ObserverSet>());
         snapshot.ObservedData = await MapObservedDataToSnapshots(project.AllObservedData);
         snapshot.Simulations = await mapSimulationsToSnapshots(project.All<ModelSimulation>(), project);
         snapshot.ParameterIdentifications = await MapParameterIdentificationToSnapshots(project.AllParameterIdentifications);
         snapshot.SimulationComparisons = await mapSimulationComparisonsToSnapshots(project.AllSimulationComparisons);
         snapshot.QualificationPlans = await mapQualificationPlansToSnapshots(project.AllQualificationPlans);
         snapshot.ObservedDataClassifications = await MapClassifications<ClassifiableObservedData>(project);
         snapshot.SimulationComparisonClassifications = await MapClassifications<ClassifiableComparison>(project);
         snapshot.SimulationClassifications = await MapClassifications<ClassifiableSimulation>(project);
         snapshot.ParameterIdentificationClassifications = await MapClassifications<ClassifiableParameterIdentification>(project);
         snapshot.QualificationPlanClassifications = await MapClassifications<ClassifiableQualificationPlan>(project);
         snapshot.CompoundClassifications = await MapClassifications<ClassifiableCompound>(project);
         snapshot.FormulationClassifications = await MapClassifications<ClassifiableFormulation>(project);
         snapshot.IndividualClassifications = await MapClassifications<ClassifiableIndividual>(project);
         snapshot.PopulationClassifications = await MapClassifications<ClassifiablePopulation>(project);
         snapshot.ProtocolClassifications = await MapClassifications<ClassifiableProtocol>(project);
         snapshot.EventClassifications = await MapClassifications<ClassifiableEvent>(project);
         snapshot.ObserverSetClassifications = await MapClassifications<ClassifiableObserverSet>(project);
         snapshot.ExpressionProfileClassifications = await MapClassifications<ClassifiableExpressionProfile>(project);
         return snapshot;
      }

      public override async Task<ModelProject> MapToModel(SnapshotProject projectSnapshot, ProjectContext projectContext)
      {
         _logger.AddDebug(PKSimConstants.UI.LoadingProjectFromSnapshot(projectSnapshot.Name), projectSnapshot.Name);

         var project = new ModelProject
         {
            Name = projectSnapshot.Name,
            Description = projectSnapshot.Description,
            Creation = _creationMetaDataFactory.Create()
         };

         //The entry point of our context structure. 
         Debug.WriteLine(projectSnapshot.Version);
         var snapshotContext = new SnapshotContext(project, SnapshotVersions.FindByPKSimProjectVersion(projectSnapshot.Version));

         project.Creation.InternalVersion = projectSnapshot.Version;
         project.Creation.Version = ProjectVersions.FindBy(projectSnapshot.Version)?.VersionDisplay;

         await allBuildingBlocksFrom(projectSnapshot, snapshotContext);

         var observedData = await ObservedDataFrom(projectSnapshot.ObservedData, snapshotContext);
         observedData?.Each(repository => AddObservedDataToProject(project, repository));

         var allSimulationsWithSnapshots = await allSimulationsFrom(projectContext, projectSnapshot.Simulations, snapshotContext);
         allSimulationsWithSnapshots?.Each(x => addSimulationToProject(project, x.simulation));

         var allSimulationComparisons = await allSimulationComparisonsFrom(projectSnapshot.SimulationComparisons, snapshotContext);
         allSimulationComparisons?.Each(comparison => addComparisonToProject(project, comparison));

         var allParameterIdentifications = await AllParameterIdentificationsFrom(projectSnapshot.ParameterIdentifications, snapshotContext);
         allParameterIdentifications?.Each(parameterIdentification => AddParameterIdentificationToProject(project, parameterIdentification));

         var allQualificationPlans = await allQualificationPlansFrom(projectSnapshot.QualificationPlans, snapshotContext);
         allQualificationPlans?.Each(qualificationPlan => addQualificationPlanToProject(project, qualificationPlan));

         if (projectContext.RunSimulations)
         {
            await runParallelSimulations(allSimulationsWithSnapshots, snapshotContext);
         }

         await addAnalysesToSimulations(snapshotContext, allSimulationsWithSnapshots, projectContext.RunSimulations);

         //Map all classifications once project is loaded
         await updateProjectClassifications(projectSnapshot, snapshotContext);

         return project;
      }

      private async Task addAnalysesToSimulations(SnapshotContext snapshotContext, IReadOnlyList<(ModelSimulation simulation, Simulation snapshotSimulation)> allSimulations, bool runSimulations)
      {
         var simulationContext = new SimulationContext(runSimulations, snapshotContext)
         {
            NumberOfSimulationsToLoad = allSimulations.Count,
            NumberOfSimulationsLoaded = 1
         };

         foreach (var simulationWithSnapshot in allSimulations)
         {
            var (simulation, snapshot) = simulationWithSnapshot;
            simulation.AddAnalyses(await individualAnalysesFrom(simulation, snapshot.IndividualAnalyses, simulationContext));
            simulation.AddAnalyses(await populationAnalysesFrom(simulation, snapshot.PopulationAnalyses, simulationContext));
         }
      }

      private async Task runParallelSimulations(IReadOnlyList<(ModelSimulation, Simulation)> simulationsWithSnapshot, SnapshotContext snapshotContext)
      {
         if (!simulationsWithSnapshot.Any())
            return;

         var options = new ParallelOptions
         {
            MaxDegreeOfParallelism = Math.Max(1, _userSettings.MaximumNumberOfCoresToUse)
         };

         var allSimCount = simulationsWithSnapshot.Count;
         _logger.AddInfo(PKSimConstants.UI.SimulationRunningMessage(allSimCount));
         await Parallel.ForEachAsync(simulationsWithSnapshot, options, async (simulationWithSnapshot, ct) =>
         {
            try
            {
               var (simulation, _) = simulationWithSnapshot;
               await _simulationRunner.RunSimulation(simulation, cancellationToken: ct);
               var remaining = System.Threading.Interlocked.Decrement(ref allSimCount);
               _logger.AddInfo(PKSimConstants.UI.SimulationFinishedMessage(simulation.Name, remaining));
            }
            catch (Exception ex)
            {
               _logger.AddException(ex);
            }
         });
         _logger.AddInfo(PKSimConstants.UI.AllSimulationsFinishedMessage());
      }

      private Task<ISimulationComparison[]> allSimulationComparisonsFrom(SimulationComparison[] snapshotSimulationComparisons, SnapshotContext snapshotContext)
         => _simulationComparisonMapper.MapToModels(snapshotSimulationComparisons, snapshotContext);

      private Task<SimulationTimeProfileChart[]> individualAnalysesFrom(ModelSimulation simulation, CurveChart[] snapshotCharts, SimulationContext simulationContext)
      {
         return analysesFrom(simulation, snapshotCharts, simulationContext, _simulationTimeProfileChartMapper.MapToModels);
      }

      public Task<ModelPopulationAnalysisChart[]> populationAnalysesFrom(ModelSimulation simulation, PopulationAnalysisChart[] snapshotPopulationAnalyses, SimulationContext simulationContext)
      {
         return analysesFrom(simulation, snapshotPopulationAnalyses, simulationContext, _populationAnalysisChartMapper.MapToModels);
      }

      private Task<TAnalysis[]> analysesFrom<TAnalysis, TSnapshotAnalysis>(ModelSimulation simulation, TSnapshotAnalysis[] snapshotAnalyses, SimulationContext simulationContext,
         Func<TSnapshotAnalysis[], SimulationAnalysisContext, Task<TAnalysis[]>> mapFunc)
      {
         if (snapshotAnalyses == null)
            return Task.FromResult(new List<TAnalysis>().ToArray());

         var project = simulationContext.Project;
         var curveChartContext = new SimulationAnalysisContext(project.AllObservedData, simulationContext) { RunSimulation = simulationContext.Run };

         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation?.DataRepository != null)
            curveChartContext.AddDataRepository(individualSimulation.DataRepository);

         return mapFunc(snapshotAnalyses, curveChartContext);
      }

      private Task<OSPSuite.Core.Domain.QualificationPlan[]> allQualificationPlansFrom(QualificationPlan[] qualificationPlans, SnapshotContext snapshotContext)
         => _qualificationPlanMapper.MapToModels(qualificationPlans, snapshotContext);

      private Task<SimulationComparison[]> mapSimulationComparisonsToSnapshots(IReadOnlyCollection<ISimulationComparison> allSimulationComparisons)
      {
         if (!allSimulationComparisons.Any())
            return Task.FromResult<SimulationComparison[]>(null);

         allSimulationComparisons.Each(load);
         return _simulationComparisonMapper.MapToSnapshots(allSimulationComparisons);
      }

      private Task<Simulation[]> mapSimulationsToSnapshots(IReadOnlyCollection<ModelSimulation> allSimulations, ModelProject project)
      {
         allSimulations.Each(loadSimulation);
         return _simulationMapper.MapToSnapshots(allSimulations, project);
      }

      private Task<QualificationPlan[]> mapQualificationPlansToSnapshots(IReadOnlyCollection<OSPSuite.Core.Domain.QualificationPlan> allQualificationPlans)
      {
         return _qualificationPlanMapper.MapToSnapshots(allQualificationPlans);
      }

      protected override Task<ParameterIdentification[]> MapParameterIdentificationToSnapshots(IReadOnlyCollection<ModelParameterIdentification> allParameterIdentifications)
      {
         allParameterIdentifications.Each(load);
         return base.MapParameterIdentificationToSnapshots(allParameterIdentifications);
      }

      private void load(ILazyLoadable lazyLoadable) => _lazyLoadTask.Load(lazyLoadable);

      private void loadSimulation(ModelSimulation simulation)
      {
         load(simulation);
         _lazyLoadTask.LoadResults(simulation);
      }

      private Task<T[]> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         return MapModelsToSnapshot<IPKSimBuildingBlock, T>(buildingBlocks, mapBuildingBlockToSnapshot);
      }

      private Task<object> mapBuildingBlockToSnapshot(IPKSimBuildingBlock buildingBlock)
      {
         _lazyLoadTask.Load(buildingBlock);
         return SnapshotMapper.MapToSnapshot(buildingBlock);
      }

      private Task updateProjectClassifications(SnapshotProject snapshot, SnapshotContext snapshotContext)
      {
         var project = snapshotContext.PKSimProject();
         var tasks = new[]
         {
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, ModelDataRepository>(
               snapshot.ObservedDataClassifications, snapshotContext, project.AllObservedData),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, ModelSimulation>(snapshot.SimulationClassifications,
               snapshotContext, project.All<ModelSimulation>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(
               snapshot.SimulationComparisonClassifications, snapshotContext, project.AllSimulationComparisons),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableParameterIdentification, ModelParameterIdentification>(
               snapshot.ParameterIdentificationClassifications, snapshotContext, project.AllParameterIdentifications),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableQualificationPlan, OSPSuite.Core.Domain.QualificationPlan>(
               snapshot.QualificationPlanClassifications, snapshotContext, project.AllQualificationPlans),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableCompound, Model.Compound>(
               snapshot.CompoundClassifications, snapshotContext, project.All<Model.Compound>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableFormulation, Model.Formulation>(
               snapshot.FormulationClassifications, snapshotContext, project.All<Model.Formulation>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableIndividual, Model.Individual>(
               snapshot.IndividualClassifications, snapshotContext, project.All<Model.Individual>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiablePopulation, Model.Population>(
               snapshot.PopulationClassifications, snapshotContext, project.All<Model.Population>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableProtocol, Model.Protocol>(
               snapshot.ProtocolClassifications, snapshotContext, project.All<Model.Protocol>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableEvent, PKSimEvent>(
               snapshot.EventClassifications, snapshotContext, project.All<PKSimEvent>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObserverSet, Model.ObserverSet>(
               snapshot.ObserverSetClassifications, snapshotContext, project.All<Model.ObserverSet>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableExpressionProfile, Model.ExpressionProfile>(
               snapshot.ExpressionProfileClassifications, snapshotContext, project.All<Model.ExpressionProfile>()),
         };

         return Task.WhenAll(tasks);
      }

      private void addSimulationToProject(ModelProject project, ModelSimulation simulation)
      {
         AddClassifiableToProject<ClassifiableSimulation, ModelSimulation>(project, simulation, project.AddBuildingBlock, project.All<ModelSimulation>());
      }

      private void addComparisonToProject(ModelProject project, ISimulationComparison simulationComparison)
      {
         AddClassifiableToProject<ClassifiableComparison, ISimulationComparison>(project, simulationComparison, project.AddSimulationComparison, project.AllSimulationComparisons);
      }

      private void addQualificationPlanToProject(ModelProject project, OSPSuite.Core.Domain.QualificationPlan qualificationPlan)
      {
         AddClassifiableToProject<ClassifiableQualificationPlan, OSPSuite.Core.Domain.QualificationPlan>(project, qualificationPlan, project.AddQualificationPlan, project.AllQualificationPlans);
      }

      private async Task<IReadOnlyList<(ModelSimulation simulation, Simulation snapshotSimulation)>> allSimulationsFrom(ProjectContext projectContext, Simulation[] snapshots, SnapshotContext snapshotContext)
      {
         var simulations = new List<(ModelSimulation, Simulation)>();

         if (snapshots == null)
            return simulations;

         var simulationContext = new SimulationContext(projectContext.RunSimulations, snapshotContext)
         {
            NumberOfSimulationsToLoad = snapshots.Length,
            NumberOfSimulationsLoaded = 1
         };

         //do not run tasks in parallel as the same mapper instance may be used concurrently to load two different snapshots
         foreach (var snapshot in snapshots)
         {
            try
            {
               var simulation = await _simulationMapper.MapToModel(snapshot, simulationContext);
               simulations.Add(new(simulation, snapshot));
               simulationContext.NumberOfSimulationsLoaded++;
            }
            catch (Exception e)
            {
               _logger.AddException(e);
               _logger.AddError(PKSimConstants.Error.CannotLoadSimulation(snapshot.Name));
            }
         }

         return simulations;
      }

      private async Task allBuildingBlocksFrom(SnapshotProject snapshot, SnapshotContext snapshotContext)
      {
         //Expression profile needs to be added first
         var expressionProfiles = await mapSnapshotToBuildingBlocks<Model.ExpressionProfile, ExpressionProfile>(snapshot.ExpressionProfiles, snapshotContext);
         expressionProfiles.Each(snapshotContext.PKSimProject().AddBuildingBlock);

         //other can be loaded independently
         var buildingBlocks = new List<IPKSimBuildingBlock>();
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Individual, Individual>(snapshot.Individuals, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Compound, Compound>(snapshot.Compounds, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<PKSimEvent, Event>(snapshot.Events, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Formulation, Formulation>(snapshot.Formulations, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Protocol, Protocol>(snapshot.Protocols, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Population, Population>(snapshot.Populations, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.ObserverSet, ObserverSet>(snapshot.ObserverSets, snapshotContext));

         buildingBlocks.Each(snapshotContext.PKSimProject().AddBuildingBlock);
      }

      private async Task<IEnumerable<IPKSimBuildingBlock>> mapSnapshotToBuildingBlocks<TModel, TSnapshot>(IEnumerable<TSnapshot> snapshots, SnapshotContext snapshotContext) where TModel : IPKSimBuildingBlock
      {
         var buildingBlocks = new List<IPKSimBuildingBlock>();

         if (snapshots == null)
            return buildingBlocks;

         //do not run tasks in parallel as the same mapper instance may be used concurrently to load two different snapshots
         foreach (var snapshot in snapshots)
         {
            var buildingBlock = await mapSnapshotToBuildingBlock<TModel, TSnapshot>(snapshot, snapshotContext);
            if (buildingBlock == null)
               continue;

            var existingBuildingBlock = buildingBlocks.FindByName(buildingBlock.Name);

            //we have a building block with the same name? The snapshot was probably edited by hand and is corrupted
            if (existingBuildingBlock != null)
            {
               LogDuplicateEntryError(buildingBlock);
               continue;
            }

            buildingBlocks.Add(buildingBlock);
         }

         return buildingBlocks;
      }

      private async Task<IPKSimBuildingBlock> mapSnapshotToBuildingBlock<TModel, TSnapshot>(TSnapshot snapshot, SnapshotContext snapshotContext) where TModel : IPKSimBuildingBlock
      {
         if (snapshot == null)
            return null;

         var mapper = SnapshotMapper.MapperFor(snapshot);
         return await mapper.MapToModel(snapshot, snapshotContext) as IPKSimBuildingBlock;
      }
   }
}