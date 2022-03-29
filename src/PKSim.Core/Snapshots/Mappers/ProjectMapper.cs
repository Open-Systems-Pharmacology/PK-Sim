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
   public class ProjectContext
   {
      public ProjectContext(bool runSimulations)
      {
         RunSimulations = runSimulations;
      }

      public bool RunSimulations { get;  }
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


         project.Creation.InternalVersion = snapshot.Version;
         project.Creation.Version = ProjectVersions.FindBy(snapshot.Version)?.VersionDisplay;

         await allBuildingBlocksFrom(snapshot, project);

         var observedData = await observedDataFrom(snapshot.ObservedData);
         observedData?.Each(repository => addObservedDataToProject(project, repository));

         var allSimulations = await allSimulationsFrom(project, projectContext,  snapshot.Simulations);
         allSimulations?.Each(simulation => addSimulationToProject(project, simulation));

         var allSimulationComparisons = await allSimulationComparisonsFrom(snapshot.SimulationComparisons, project);
         allSimulationComparisons?.Each(comparison => addComparisonToProject(project, comparison));

         var allParameterIdentifications = await allParameterIdentificationsFrom(snapshot.ParameterIdentifications, project);
         allParameterIdentifications?.Each(parameterIdentification => addParameterIdentificationToProject(project, parameterIdentification));

         var allQualificationPlans = await allQualificationPlansFrom(snapshot.QualificationPlans, project);
         allQualificationPlans?.Each(qualificationPlan => addQualificationPlanToProject(project, qualificationPlan));

         //Map all classifications once project is loaded
         await updateProjectClassifications(snapshot, project);

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

      private Task<ModelDataRepository[]> observedDataFrom(SnapshotDataRepository[] snapshotRepositories)
      {
         return awaitAs<ModelDataRepository>(mapSnapshotsToModels(snapshotRepositories));
      }

      private Task<ISimulationComparison[]> allSimulationComparisonsFrom(SimulationComparison[] snapshotSimulationComparisons, ModelProject project)
         => _simulationComparisonMapper.MapToModels(snapshotSimulationComparisons, project);

      private Task<ModelParameterIdentification[]> allParameterIdentificationsFrom(ParameterIdentification[] snapshotParameterIdentifications,
         ModelProject project)
         => _parameterIdentificationMapper.MapToModels(snapshotParameterIdentifications, project);

      private Task<Model.QualificationPlan[]> allQualificationPlansFrom(QualificationPlan[] qualificationPlans, ModelProject project)
         => _qualificationPlanMapper.MapToModels(qualificationPlans, project);

      private async Task<SimulationComparison[]> mapSimulationComparisonsToSnapshots(
         IReadOnlyCollection<ISimulationComparison> allSimulationComparisons)
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

      private Task updateProjectClassifications(SnapshotProject snapshot, ModelProject project)
      {
         var tasks = new[]
         {
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, ModelDataRepository>(
               snapshot.ObservedDataClassifications, project, project.AllObservedData),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, Model.Simulation>(snapshot.SimulationClassifications,
               project, project.All<Model.Simulation>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(
               snapshot.SimulationComparisonClassifications, project, project.AllSimulationComparisons),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableParameterIdentification, ModelParameterIdentification>(
               snapshot.ParameterIdentificationClassifications, project, project.AllParameterIdentifications),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableQualificationPlan, Model.QualificationPlan>(
               snapshot.QualificationPlanClassifications, project, project.AllQualificationPlans),
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

      private async Task<IEnumerable<Model.Simulation>> allSimulationsFrom(ModelProject project, ProjectContext projectContext, Simulation[] snapshots)
      {
         var simulations = new List<Model.Simulation>();

         if (snapshots == null)
            return simulations;

         var simulationContext = new SimulationContext(project, projectContext.RunSimulations);
         simulationContext.NumberOfSimulationsToLoad = snapshots.Length;
         simulationContext.NumberOfSimulationsLoaded = 1;
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

      private async Task allBuildingBlocksFrom(SnapshotProject snapshot, ModelProject project)
      {
         //Expression profile needs to be added first
         var expressionProfiles = await mapSnapshotToBuildingBlocks<Model.ExpressionProfile, ExpressionProfile>(snapshot.ExpressionProfiles, project);
         expressionProfiles.Each(project.AddBuildingBlock);

         //other can be loaded independently
         var buildingBlocks = new List<IPKSimBuildingBlock>();
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Individual, Individual>(snapshot.Individuals, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Compound, Compound>(snapshot.Compounds, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<PKSimEvent, Event>(snapshot.Events, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Formulation, Formulation>(snapshot.Formulations, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Protocol, Protocol>(snapshot.Protocols, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.Population, Population>(snapshot.Populations, project));
         buildingBlocks.AddRange(await mapSnapshotToBuildingBlocks<Model.ObserverSet, ObserverSet>(snapshot.ObserverSets, project));

         buildingBlocks.Each(project.AddBuildingBlock);
      }

      private async Task<T[]> awaitAs<T>(IEnumerable<Task<object>> tasks)
      {
         var models = await Task.WhenAll(tasks);
         var array = models.OfType<T>().ToArray();
         return array.Any() ? array : null;
      }

      private IEnumerable<Task<object>> mapSnapshotsToModels(IEnumerable<object> snapshots)
      {
         if (snapshots == null)
            return Enumerable.Empty<Task<object>>();

         return snapshots.Select(snapshotMapper.MapToModel);
      }

      private async Task<IEnumerable<IPKSimBuildingBlock>> mapSnapshotToBuildingBlocks<TModel, TSnapshot>(IEnumerable<TSnapshot> snapshots, ModelProject project) where TModel : IPKSimBuildingBlock
      {
         var buildingBlocks = new List<IPKSimBuildingBlock>();

         if (snapshots == null)
            return buildingBlocks;

         //do not run tasks in parallel as the same mapper instance may be used concurrently to load two different snapshots
         foreach (var snapshot in snapshots)
         {
            var buildingBlock = await mapSnapshotToBuildingBlock<TModel, TSnapshot>(snapshot, project);
            if (buildingBlock != null)
               buildingBlocks.Add(buildingBlock);
         }

         return buildingBlocks;
      }

      private async Task<IPKSimBuildingBlock> mapSnapshotToBuildingBlock<TModel, TSnapshot>(TSnapshot snapshot, ModelProject project) where TModel : IPKSimBuildingBlock
      {
         if (snapshot == null)
            return null;

         var mapper = snapshotMapper.MapperFor(snapshot);
         switch (mapper)
         {
            case ISnapshotMapperWithProjectAsContext<TModel, TSnapshot> mapperWithProject:
               return await mapperWithProject.MapToModel(snapshot, project);
            default:
               var buildingBlock = await mapper.MapToModel(snapshot);
               return buildingBlock.DowncastTo<IPKSimBuildingBlock>();
         }
      }

      private ISnapshotMapper snapshotMapper => _snapshotMapper.Value;
   }
}