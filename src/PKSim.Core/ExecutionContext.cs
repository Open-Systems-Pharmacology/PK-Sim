using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Core
{
   public interface IExecutionContext : IOSPSuiteExecutionContext<PKSimProject>
   {
      ICloneManager CloneManager { get; }
      void Load(ILazyLoadable lazyLoadable);
      string DisplayNameFor(IObjectBase objectBase);
      IPKSimBuildingBlock BuildingBlockContaining(IEntity entity);
      string BuildingBlockIdContaining(IEntity entity);
      void UpdateBuildingBlockVersion(IBuildingBlockChangeCommand buildingBlockChangeCommand);
      void UpdateBuildinBlockProperties(IPKSimCommand command, IPKSimBuildingBlock buildingBlock);
      void UpdateDependenciesOn(IParameter parameter);
      string ReportFor<T>(T objectToReport);
   }

   public class ExecutionContext : IExecutionContext
   {
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IWithIdRepository _withIdRepository;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IRegistrationTask _registrationTask;
      private readonly IEventPublisher _eventPublisher;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly IBuildingBlockRetriever _buildingBlockRetriever;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly IBuildingBlockVersionUpdater _buildingBlockVersionUpdater;
      private readonly IProjectChangedNotifier _projectChangedNotifier;
      private readonly IContainer _container;
      private readonly IReportGenerator _reportGenerator;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IParameterChangeUpdater _parameterChangeUpdater;
      public ICloneManager CloneManager { get; }

      public ExecutionContext(IPKSimProjectRetriever projectRetriever, IWithIdRepository withIdRepository,
         ILazyLoadTask lazyLoadTask, IRegistrationTask registrationTask,
         IEventPublisher eventPublisher, IObjectTypeResolver objectTypeResolver,
         IBuildingBlockRetriever buildingBlockRetriever, ICompressedSerializationManager serializationManager,
         IBuildingBlockVersionUpdater buildingBlockVersionUpdater, IProjectChangedNotifier projectChangedNotifier,
         ICloner cloner, IContainer container,
         IReportGenerator reportGenerator,
         IFullPathDisplayResolver fullPathDisplayResolver,
         IParameterChangeUpdater parameterChangeUpdater)
      {
         _projectRetriever = projectRetriever;
         _withIdRepository = withIdRepository;
         _lazyLoadTask = lazyLoadTask;
         _registrationTask = registrationTask;
         _eventPublisher = eventPublisher;
         _objectTypeResolver = objectTypeResolver;
         _buildingBlockRetriever = buildingBlockRetriever;
         _serializationManager = serializationManager;
         _buildingBlockVersionUpdater = buildingBlockVersionUpdater;
         _projectChangedNotifier = projectChangedNotifier;
         CloneManager = cloner;
         _container = container;
         _reportGenerator = reportGenerator;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _parameterChangeUpdater = parameterChangeUpdater;
      }

      public PKSimProject CurrentProject => _projectRetriever.CurrentProject.DowncastTo<PKSimProject>();

      public T Get<T>(string id) where T : class, IWithId
      {
         return Get(id) as T;
      }

      public IWithId Get(string id)
      {
         if (!_withIdRepository.ContainsObjectWithId(id))
            return null;

         var entity = _withIdRepository.Get(id);
         if (entity == null)
            return null;

         Load(entity as ILazyLoadable);

         return entity;
      }

      public void Load(IObjectBase objectBase)
      {
         Load(objectBase as ILazyLoadable);
      }

      public void Load(ILazyLoadable lazyLoadable)
      {
         _lazyLoadTask.Load(lazyLoadable);
      }

      public void Register(IWithId objectToRegister)
      {
         _registrationTask.Register(objectToRegister);
      }

      public void Unregister(IWithId objectToUnregister)
      {
         _registrationTask.Unregister(objectToUnregister);
      }

      public T Resolve<T>()
      {
         return _container.Resolve<T>();
      }

      public void PublishEvent<TEvent>(TEvent eventToPublish)
      {
         _eventPublisher.PublishEvent(eventToPublish);
      }

      public string TypeFor<T>(T entity) where T : class
      {
         return _objectTypeResolver.TypeFor(entity);
      }

      public IPKSimBuildingBlock BuildingBlockContaining(IEntity entity)
      {
         return _buildingBlockRetriever.BuildingBlockContaining(entity);
      }

      public string BuildingBlockIdContaining(IEntity entity)
      {
         return _buildingBlockRetriever.BuildingBlockIdContaining(entity);
      }

      public byte[] Serialize<TObject>(TObject objectToSerialize)
      {
         return _serializationManager.Serialize(objectToSerialize);
      }

      public TObject Deserialize<TObject>(byte[] serializationByte)
      {
         return _serializationManager.Deserialize<TObject>(serializationByte);
      }

      public T Clone<T>(T objectToClone) where T : class, IObjectBase
      {
         return CloneManager.Clone(objectToClone);
      }

      public void UpdateBuildingBlockVersion(IBuildingBlockChangeCommand buildingBlockChangeCommand)
      {
         var buildingBlock = Get<IPKSimBuildingBlock>(buildingBlockChangeCommand.BuildingBlockId);
         _buildingBlockVersionUpdater.UpdateBuildingBlockVersion(buildingBlockChangeCommand, buildingBlock);
         if (buildingBlock == null) return;
         buildingBlock.HasChanged = true;
      }

      public void UpdateBuildinBlockProperties(IPKSimCommand command, IPKSimBuildingBlock buildingBlock)
      {
         if (buildingBlock == null)
         {
            command.BuildingBlockType = CoreConstants.ContainerName.TypeTemplate;
            command.BuildingBlockName = CoreConstants.ContainerName.NameTemplate;
            return;
         }

         command.BuildingBlockType = TypeFor(buildingBlock);
         command.BuildingBlockName = string.IsNullOrEmpty(buildingBlock.Name) ? CoreConstants.ContainerName.NameTemplate : buildingBlock.Name;
      }

      public void UpdateDependenciesOn(IParameter parameter)
      {
         _parameterChangeUpdater.UpdateObjectsDependingOn(parameter);
      }

      public void ProjectChanged()
      {
         _projectChangedNotifier.Changed();
      }

      public string ReportFor<T>(T objectToReport)
      {
         return _reportGenerator.StringReportFor(objectToReport);
      }

      public IProject Project => CurrentProject;

      public void AddToHistory(ICommand command)
      {
         _projectRetriever.AddToHistory(command);
      }

      public string DisplayNameFor(IObjectBase objectBase)
      {
         return _fullPathDisplayResolver.FullPathFor(objectBase);
      }
   }
}