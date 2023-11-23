using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;
using ModelParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectContext : SnapshotContext
   {
      public ProjectContext(bool runSimulations)
      {
         RunSimulations = runSimulations;
      }

      public bool RunSimulations { get; }
   }

   public class ProjectMapper : SnapshotMapperBase<ModelProject, SnapshotProject, ProjectContext>
   {
      private readonly SimulationMapper _simulationMapper;
      private readonly SimulationComparisonMapper _simulationComparisonMapper;
      private readonly ParameterIdentificationMapper _parameterIdentificationMapper;
      private readonly QualificationPlanMapper _qualificationPlanMapper;
      private readonly IClassificationSnapshotTask _classificationSnapshotTask;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly ICreationMetaDataFactory _creationMetaDataFactory;
      private readonly IOSPSuiteLogger _logger;
      private readonly Lazy<ISnapshotMapper> _snapshotMapper;

      public ProjectMapper(
         SimulationMapper simulationMapper,
         SimulationComparisonMapper simulationComparisonMapper,
         ParameterIdentificationMapper parameterIdentificationMapper,
         QualificationPlanMapper qualificationPlanMapper,
         IExecutionContext executionContext,
         IClassificationSnapshotTask classificationSnapshotTask,
         ILazyLoadTask lazyLoadTask,
         ICreationMetaDataFactory creationMetaDataFactory,
         IOSPSuiteLogger logger)
      {
         _simulationMapper = simulationMapper;
         _simulationComparisonMapper = simulationComparisonMapper;
         _parameterIdentificationMapper = parameterIdentificationMapper;
         _qualificationPlanMapper = qualificationPlanMapper;
         _classificationSnapshotTask = classificationSnapshotTask;
         _lazyLoadTask = lazyLoadTask;
         _creationMetaDataFactory = creationMetaDataFactory;
         _logger = logger;
         //required to load the snapshot mapper via execution context to avoid circular references
         _snapshotMapper = new Lazy<ISnapshotMapper>(executionContext.Resolve<ISnapshotMapper>);
      }

      public override async Task<SnapshotProject> MapToSnapshot(ModelProject project)
      {
         var snapshot = await SnapshotFrom(project, x =>
         {
            x.Version = ProjectVersions.Current;
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
         snapshot.ObservedData = await mapObservedDataToSnapshots(project.AllObservedData);
         snapshot.Simulations = await mapSimulationsToSnapshots(project.All<Model.Simulation>(), project);
         snapshot.ParameterIdentifications = await mapParameterIdentificationToSnapshots(project.AllParameterIdentifications);
         snapshot.SimulationComparisons = await mapSimulationComparisonsToSnapshots(project.AllSimulationComparisons);
         snapshot.QualificationPlans = await mapQualificationPlansToSnapshots(project.AllQualificationPlans);
         snapshot.ObservedDataClassifications = await mapClassifications<ClassifiableObservedData>(project);
         snapshot.SimulationComparisonClassifications = await mapClassifications<ClassifiableComparison>(project);
         snapshot.SimulationClassifications = await mapClassifications<ClassifiableSimulation>(project);
         snapshot.ParameterIdentificationClassifications = await mapClassifications<ClassifiableParameterIdentification>(project);
         snapshot.QualificationPlanClassifications = await mapClassifications<ClassifiableQualificationPlan>(project);
         return snapshot;
      }

      public override async Task<ModelProject> MapToModel(SnapshotProject snapshot, ProjectContext projectContext)
      {
         _logger.AddDebug($"Loading project '{snapshot.Name}' from snapshot...", snapshot.Name);

         var project = new ModelProject
         {
            Name = snapshot.Name,
            Description = snapshot.Description,
            Creation = _creationMetaDataFactory.Create()
         };

         //The entry point of our context structure. 
         var snapshotContext = new SnapshotContext(project, snapshot.Version);

         project.Creation.InternalVersion = snapshot.Version;
         project.Creation.Version = ProjectVersions.FindBy(snapshot.Version)?.VersionDisplay;

         await allBuildingBlocksFrom(snapshot, snapshotContext);

         var observedData = await observedDataFrom(snapshot.ObservedData, snapshotContext);
         observedData?.Each(repository => addObservedDataToProject(project, repository));

         var allSimulations = await allSimulationsFrom(projectContext, snapshot.Simulations, snapshotContext);
         allSimulations?.Each(simulation => addSimulationToProject(project, simulation));

         var allSimulationComparisons = await allSimulationComparisonsFrom(snapshot.SimulationComparisons, snapshotContext);
         allSimulationComparisons?.Each(comparison => addComparisonToProject(project, comparison));

         var allParameterIdentifications = await allParameterIdentificationsFrom(snapshot.ParameterIdentifications, snapshotContext);
         allParameterIdentifications?.Each(parameterIdentification => addParameterIdentificationToProject(project, parameterIdentification));

         var allQualificationPlans = await allQualificationPlansFrom(snapshot.QualificationPlans, snapshotContext);
         allQualificationPlans?.Each(qualificationPlan => addQualificationPlanToProject(project, qualificationPlan));

         //Map all classifications once project is loaded
         await updateProjectClassifications(snapshot, snapshotContext);

         return project;
      }

      private Task<Classification[]> mapClassifications<TClassifiable>(ModelProject project) where TClassifiable : class, IClassifiableWrapper, new()
      {
         return _classificationSnapshotTask.MapClassificationsToSnapshots<TClassifiable>(project);
      }

      private Task<SnapshotDataRepository[]> mapObservedDataToSnapshots(IReadOnlyCollection<ModelDataRepository> allObservedData)
      {
         return mapModelsToSnapshot<ModelDataRepository, SnapshotDataRepository>(allObservedData, snapshotMapper.MapToSnapshot);
      }

      private Task<ModelDataRepository[]> observedDataFrom(DataRepository[] snapshotRepositories, SnapshotContext snapshotContext)
      {
         return awaitAs<ModelDataRepository>(mapSnapshotsToModels(snapshotRepositories, snapshotContext));
      }

      private Task<ISimulationComparison[]> allSimulationComparisonsFrom(SimulationComparison[] snapshotSimulationComparisons, SnapshotContext snapshotContext)
         => _simulationComparisonMapper.MapToModels(snapshotSimulationComparisons, snapshotContext);

      private Task<ModelParameterIdentification[]> allParameterIdentificationsFrom(ParameterIdentification[] snapshotParameterIdentifications,
         SnapshotContext snapshotContext)
         => _parameterIdentificationMapper.MapToModels(snapshotParameterIdentifications, snapshotContext);

      private Task<Model.QualificationPlan[]> allQualificationPlansFrom(QualificationPlan[] qualificationPlans, SnapshotContext snapshotContext)
         => _qualificationPlanMapper.MapToModels(qualificationPlans, snapshotContext);

      private async Task<SimulationComparison[]> mapSimulationComparisonsToSnapshots(IReadOnlyCollection<ISimulationComparison> allSimulationComparisons)
      {
         if (!allSimulationComparisons.Any())
            return null;

         allSimulationComparisons.Each(load);
         return await _simulationComparisonMapper.MapToSnapshots(allSimulationComparisons);
      }

      private async Task<Simulation[]> mapSimulationsToSnapshots(IReadOnlyCollection<Model.Simulation> allSimulations, ModelProject project)
      {
         allSimulations.Each(loadSimulation);
         return await _simulationMapper.MapToSnapshots(allSimulations, project);
      }

      private async Task<QualificationPlan[]> mapQualificationPlansToSnapshots(IReadOnlyCollection<Model.QualificationPlan> allQualificationPlans)
      {
         return await _qualificationPlanMapper.MapToSnapshots(allQualificationPlans);
      }

      private async Task<ParameterIdentification[]> mapParameterIdentificationToSnapshots(IReadOnlyCollection<ModelParameterIdentification> allParameterIdentifications)
      {
         allParameterIdentifications.Each(load);
         return await _parameterIdentificationMapper.MapToSnapshots(allParameterIdentifications);
      }

      private void load(ILazyLoadable lazyLoadable) => _lazyLoadTask.Load(lazyLoadable);

      private void loadSimulation(Model.Simulation simulation)
      {
         load(simulation);
         _lazyLoadTask.LoadResults(simulation);
      }

      private Task<T[]> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         return mapModelsToSnapshot<IPKSimBuildingBlock, T>(buildingBlocks, mapBuildingBlockToSnapshot);
      }

      private Task<object> mapBuildingBlockToSnapshot(IPKSimBuildingBlock buildingBlock)
      {
         _lazyLoadTask.Load(buildingBlock);
         return snapshotMapper.MapToSnapshot(buildingBlock);
      }

      private Task<TSnapshot[]> mapModelsToSnapshot<TModel, TSnapshot>(IEnumerable<TModel> models, Func<TModel, Task<object>> mapFunc)
      {
         var tasks = models.Select(mapFunc);
         return awaitAs<TSnapshot>(tasks);
      }

      private Task updateProjectClassifications(SnapshotProject snapshot, SnapshotContext snapshotContext)
      {
         var project = snapshotContext.Project;
         var tasks = new[]
         {
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, ModelDataRepository>(
               snapshot.ObservedDataClassifications, snapshotContext, project.AllObservedData),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, Model.Simulation>(snapshot.SimulationClassifications,
               snapshotContext, project.All<Model.Simulation>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(
               snapshot.SimulationComparisonClassifications, snapshotContext, project.AllSimulationComparisons),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableParameterIdentification, ModelParameterIdentification>(
               snapshot.ParameterIdentificationClassifications, snapshotContext, project.AllParameterIdentifications),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableQualificationPlan, Model.QualificationPlan>(
               snapshot.QualificationPlanClassifications, snapshotContext, project.AllQualificationPlans),
         };

         return Task.WhenAll(tasks);
      }

      private void addSimulationToProject(ModelProject project, Model.Simulation simulation)
      {
         addClassifiableToProject<ClassifiableSimulation, Model.Simulation>(project, simulation, project.AddBuildingBlock);
      }

      private void addObservedDataToProject(ModelProject project, ModelDataRepository repository)
      {
         addClassifiableToProject<ClassifiableObservedData, ModelDataRepository>(project, repository, project.AddObservedData);
      }

      private void addComparisonToProject(ModelProject project, ISimulationComparison simulationComparison)
      {
         addClassifiableToProject<ClassifiableComparison, ISimulationComparison>(project, simulationComparison, project.AddSimulationComparison);
      }

      private void addParameterIdentificationToProject(ModelProject project, ModelParameterIdentification parameterIdentification)
      {
         addClassifiableToProject<ClassifiableParameterIdentification, ModelParameterIdentification>(project, parameterIdentification,
            project.AddParameterIdentification);
      }

      private void addQualificationPlanToProject(ModelProject project, Model.QualificationPlan qualificationPlan)
      {
         addClassifiableToProject<ClassifiableQualificationPlan, Model.QualificationPlan>(project, qualificationPlan, project.AddQualificationPlan);
      }

      private void addClassifiableToProject<TClassifiableWrapper, TSubject>(ModelProject project, TSubject subject,
         Action<TSubject> addToProjectAction) where TClassifiableWrapper : Classifiable<TSubject>, new() where TSubject : IWithId, IWithName
      {
         addToProjectAction(subject);
         project.GetOrCreateClassifiableFor<TClassifiableWrapper, TSubject>(subject);
      }

      private async Task<IReadOnlyList<Model.Simulation>> allSimulationsFrom(ProjectContext projectContext, Simulation[] snapshots, SnapshotContext snapshotContext)
      {
         var simulations = new List<Model.Simulation>();

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
               simulations.Add(simulation);
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
         expressionProfiles.Each(snapshotContext.Project.AddBuildingBlock);

         //other can be loaded independently
         var buildingBlocks = new List<IPKSimBuildingBlock>();
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Individual, Individual>(snapshot.Individuals, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Compound, Compound>(snapshot.Compounds, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<PKSimEvent, Event>(snapshot.Events, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Formulation, Formulation>(snapshot.Formulations, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Protocol, Protocol>(snapshot.Protocols, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Population, Population>(snapshot.Populations, snapshotContext));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.ObserverSet, ObserverSet>(snapshot.ObserverSets, snapshotContext));

         buildingBlocks.Each(snapshotContext.Project.AddBuildingBlock);
      }

      private async Task<T[]> awaitAs<T>(IEnumerable<Task<object>> tasks)
      {
         var models = await Task.WhenAll(tasks);
         var array = models.OfType<T>().ToArray();
         return array.Any() ? array : null;
      }

      private IEnumerable<Task<object>> mapSnapshotsToModels(IEnumerable<object> snapshots, SnapshotContext snapshotContext)
      {
         if (snapshots == null)
            return Enumerable.Empty<Task<object>>();

         return snapshots.Select(x => snapshotMapper.MapToModel(x, snapshotContext));
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
            if (buildingBlock != null)
               buildingBlocks.Add(buildingBlock);
         }

         return buildingBlocks;
      }

      private async Task<IPKSimBuildingBlock> mapSnapshotToBuildingBlock<TModel, TSnapshot>(TSnapshot snapshot, SnapshotContext snapshotContext) where TModel : IPKSimBuildingBlock
      {
         if (snapshot == null)
            return null;

         var mapper = snapshotMapper.MapperFor(snapshot);
         return await mapper.MapToModel(snapshot, snapshotContext) as IPKSimBuildingBlock;
      }

      private ISnapshotMapper snapshotMapper => _snapshotMapper.Value;
   }
}