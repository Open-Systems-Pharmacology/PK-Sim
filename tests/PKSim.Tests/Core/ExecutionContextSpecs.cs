using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Core
{
   public abstract class concern_for_ExecutionContext : ContextSpecification<IExecutionContext>
   {
      protected IPKSimProjectRetriever _projectRetriever;
      protected PKSimProject _project;
      protected IWithIdRepository _withIdRepository;
      protected IParameter _parameter;
      protected ILazyLoadTask _lazyLoadTask;
      protected string _idThatDoesNotExist;
      protected string _idThatDoesExist;
      protected IRegistrationTask _registrationTask;
      protected IEventPublisher _eventPublisher;
      protected IObjectTypeResolver _objectTypeResolver;
      protected IBuildingBlockRetriever _buildingBlockRetriever;
      protected IBuildingBlockVersionUpdater _buildingBlockVersionUpdater;
      protected IProjectChangedNotifier _projectChangedNotifier;
      protected ICloner _cloneManager;
      protected IContainer _container;
      protected IReportGenerator _reportGenerator;
      protected IFullPathDisplayResolver _fullPathDisplayResolver;
      protected ICompressedSerializationManager _stringSerializer;
      private IParameterChangeUpdater _parameterChangeUpdater;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _buildingBlockRetriever = A.Fake<IBuildingBlockRetriever>();
         _buildingBlockVersionUpdater = A.Fake<IBuildingBlockVersionUpdater>();
         _projectChangedNotifier = A.Fake<IProjectChangedNotifier>();
         _withIdRepository = A.Fake<IWithIdRepository>();
         _stringSerializer = A.Fake<ICompressedSerializationManager>();
         _cloneManager = A.Fake<ICloner>();
         _reportGenerator = A.Fake<IReportGenerator>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _project = A.Fake<PKSimProject>();
         _idThatDoesNotExist = "tralalalal";
         _parameter = A.Fake<IParameter>();
         _parameterChangeUpdater= A.Fake<IParameterChangeUpdater>();
         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
         _idThatDoesExist = "toto";
         _container = A.Fake<IContainer>();
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_idThatDoesExist)).Returns(true);
         A.CallTo(() => _withIdRepository.Get<IParameter>(_idThatDoesExist)).Returns(_parameter);
         A.CallTo(() => _withIdRepository.Get(_idThatDoesExist)).Returns(_parameter);
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_idThatDoesNotExist)).Returns(false);
         A.CallTo(() => _withIdRepository.Get(_idThatDoesNotExist)).Throws(new Exception());

         sut = new ExecutionContext(_projectRetriever, _withIdRepository, _lazyLoadTask, _registrationTask,
            _eventPublisher, _objectTypeResolver, _buildingBlockRetriever,
            _stringSerializer, _buildingBlockVersionUpdater, _projectChangedNotifier,
            _cloneManager, _container, _reportGenerator, _fullPathDisplayResolver, _parameterChangeUpdater);
      }
   }

   public class When_retrieving_the_project : concern_for_ExecutionContext
   {
      [Observation]
      public void should_return_the_active_project()
      {
         sut.CurrentProject.ShouldBeEqualTo(_projectRetriever.CurrentProject);
      }
   }

   public class When_retrieving_a_typed_object_by_id : concern_for_ExecutionContext
   {
      [Observation]
      public void should_return_an_object_registered_with_the_id_and_with_the_accurate_type()
      {
         sut.Get<IParameter>(_idThatDoesExist).ShouldBeEqualTo(_parameter);
      }
   }

   public class When_retrieving_a_typed_object_by_id_that_does_not_exist : concern_for_ExecutionContext
   {
      [Observation]
      public void should_return_null()
      {
         sut.Get<IParameter>(_idThatDoesNotExist).ShouldBeNull();
      }
   }

   public class When_retrieving_an_object_by_id : concern_for_ExecutionContext
   {
      [Observation]
      public void should_return_any_object_registered_with_the_id()
      {
         sut.Get(_idThatDoesExist).ShouldBeEqualTo(_parameter);
      }
   }

   public class When_retrieving_a_lazy_loadable_typed_object_with_an_id : concern_for_ExecutionContext
   {
      private IPKSimBuildingBlock _lazyLoadable;

      protected override void Context()
      {
         base.Context();
         _lazyLoadable = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _withIdRepository.ContainsObjectWithId("tata")).Returns(true);
         A.CallTo(() => _withIdRepository.Get("tata")).Returns(_lazyLoadable);
      }

      protected override void Because()
      {
         sut.Get<IPKSimBuildingBlock>("tata");
      }

      [Observation]
      public void the_object_should_have_been_completly_loaded()
      {
         A.CallTo(() => _lazyLoadTask.Load<ILazyLoadable>(_lazyLoadable)).MustHaveHappened();
      }
   }

   public class When_retrieving_a_lazy_loadable_object_by_id : concern_for_ExecutionContext
   {
      private IPKSimBuildingBlock _lazyLoadable;

      protected override void Context()
      {
         base.Context();
         _lazyLoadable = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _withIdRepository.ContainsObjectWithId("tata")).Returns(true);
         A.CallTo(() => _withIdRepository.Get("tata")).Returns(_lazyLoadable);
      }

      protected override void Because()
      {
         sut.Get("tata");
      }

      [Observation]
      public void the_object_should_have_been_completly_loaded()
      {
         A.CallTo(() => _lazyLoadTask.Load<ILazyLoadable>(_lazyLoadable)).MustHaveHappened();
      }
   }

   public class When_asked_to_register_an_object_base : concern_for_ExecutionContext
   {
      private IObjectBase _objectToRegister;

      protected override void Context()
      {
         base.Context();
         _objectToRegister = A.Fake<IObjectBase>();
      }

      protected override void Because()
      {
         sut.Register(_objectToRegister);
      }

      [Observation]
      public void should_leverage_the_registration_task_to_register_the_object()
      {
         A.CallTo(() => _registrationTask.Register(_objectToRegister)).MustHaveHappened();
      }
   }

   public class When_asked_to_unregister_an_object_base : concern_for_ExecutionContext
   {
      private IObjectBase _objectToUnregister;

      protected override void Context()
      {
         base.Context();
         _objectToUnregister = A.Fake<IObjectBase>();
      }

      protected override void Because()
      {
         sut.Unregister(_objectToUnregister);
      }

      [Observation]
      public void should_leverage_the_registration_task_to_register_the_object()
      {
         A.CallTo(() => _registrationTask.Unregister(_objectToUnregister)).MustHaveHappened();
      }
   }

   public class When_the_execution_context_is_asked_to_publish_an_event : concern_for_ExecutionContext
   {
      private IEvent _eventToPublish;

      protected override void Context()
      {
         base.Context();
         _eventToPublish = A.Fake<IEvent>();
      }

      protected override void Because()
      {
         sut.PublishEvent(_eventToPublish);
      }

      [Observation]
      public void should_leverage_the_event_publisher_and_publish_the_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(_eventToPublish)).MustHaveHappened();
      }
   }

   public class When_the_execution_context_is_resolving_the_type_of_given_object : concern_for_ExecutionContext
   {
      private IObjectBase _objectBase;

      protected override void Context()
      {
         base.Context();
         _objectBase = A.Fake<IObjectBase>();
         A.CallTo(() => _objectTypeResolver.TypeFor(_objectBase)).Returns("trala");
      }

      [Observation]
      public void should_leverage_the_entity_type_resolver_to_retrieve_the_type()
      {
         sut.TypeFor(_objectBase).ShouldBeEqualTo(_objectTypeResolver.TypeFor(_objectBase));
      }
   }

   public class When_the_execution_context_is_serializing_an_object : concern_for_ExecutionContext
   {
      private object _objectToSerialize;
      private byte[] _serializedString;

      protected override void Context()
      {
         base.Context();
         _objectToSerialize = new object();
         _serializedString = new byte[1];
         A.CallTo(() => _stringSerializer.Serialize(_objectToSerialize)).Returns(_serializedString);
      }

      [Observation]
      public void should_leverage_the_serialization_manager_to_retrieve_the_serialized_string_from_the_object()
      {
         sut.Serialize(_objectToSerialize).ShouldBeEqualTo(_serializedString);
      }
   }

   public class When_the_execution_context_is_deserializing_an_object_from_a_string : concern_for_ExecutionContext
   {
      private object _objectToDeserialize;
      private byte[] _serializedString;

      protected override void Context()
      {
         base.Context();
         _objectToDeserialize = new object();
         _serializedString = new byte[1];
         A.CallTo(() => _stringSerializer.Deserialize<object>(_serializedString, null)).Returns(_objectToDeserialize);
      }

      [Observation]
      public void should_leverage_the_serialization_manager_to_retrieve_the_deserialized_object_from_the_string()
      {
         sut.Deserialize<object>(_serializedString).ShouldBeEqualTo(_objectToDeserialize);
      }
   }

   public class When_the_execution_context_is_retrieving_the_display_name_of_an_entity : concern_for_ExecutionContext
   {
      private string _displayName;
      private IObjectBase _objectBase;

      protected override void Context()
      {
         base.Context();
         _objectBase = A.Fake<IObjectBase>();
         _displayName = "Tralala";
         A.CallTo(_fullPathDisplayResolver).WithReturnType<string>().Returns(_displayName);
      }

      [Observation]
      public void should_leverage_the_full_path_display_resolver_to_retrieve_the_display()
      {
         sut.DisplayNameFor(_objectBase).ShouldBeEqualTo(_displayName);
      }
   }

   public class When_retrieving_the_building_block_containing_an_entity : concern_for_ExecutionContext
   {
      private IPKSimBuildingBlock _parentBuildingBlock;
      private IEntity _entity;

      protected override void Context()
      {
         base.Context();
         _parentBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _entity = A.Fake<IEntity>();
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockContaining(_entity)).Returns(_parentBuildingBlock);
      }

      [Observation]
      public void should_return_the_parent_building_block_of_that_entity()
      {
         sut.BuildingBlockContaining(_entity).ShouldBeEqualTo(_parentBuildingBlock);
      }
   }

   public class When_retrieving_the_id_of_the_building_block_containing_an_entity : concern_for_ExecutionContext
   {
      private string _parentBuildingBlockId;
      private IEntity _entity;

      protected override void Context()
      {
         base.Context();
         _parentBuildingBlockId = "tralala";
         _entity = A.Fake<IEntity>();
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockIdContaining(_entity)).Returns(_parentBuildingBlockId);
      }

      [Observation]
      public void should_return_the_id_of_the_parent_building_block_of_that_entity()
      {
         sut.BuildingBlockIdContaining(_entity).ShouldBeEqualTo(_parentBuildingBlockId);
      }
   }

   public class When_asked_to_update_the_building_block_version_for_a_given_command : concern_for_ExecutionContext
   {
      private string _id;
      private IBuildingBlockChangeCommand _command;
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _id = "tralala";
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _command = A.Fake<IBuildingBlockChangeCommand>();
         A.CallTo(() => _command.BuildingBlockId).Returns(_id);
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_id)).Returns(true);
         A.CallTo(() => _withIdRepository.Get(_id)).Returns(_buildingBlock);
      }

      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_command);
      }

      [Observation]
      public void should_retrieve_the_building_block_and_leverage_the_building_block_update()
      {
         A.CallTo(() => _buildingBlockVersionUpdater.UpdateBuildingBlockVersion(_command, _buildingBlock)).MustHaveHappened();
      }
   }

   public class When_notify_a_project_changed : concern_for_ExecutionContext
   {
      protected override void Because()
      {
         sut.ProjectChanged();
      }

      [Observation]
      public void should_leverage_the_project_change_notifier()
      {
         A.CallTo(() => _projectChangedNotifier.Changed()).MustHaveHappened();
      }
   }

   public class When_the_execution_context_is_asked_to_clone_an_object : concern_for_ExecutionContext
   {
      private IObjectBase _origin;
      private IObjectBase _clone;
      private IObjectBase _result;

      protected override void Context()
      {
         base.Context();
         _origin = A.Fake<IObjectBase>();
         _clone = A.Fake<IObjectBase>();
         A.CallTo(() => _cloneManager.Clone(_origin)).Returns(_clone);
      }

      protected override void Because()
      {
         _result = sut.Clone(_origin);
      }

      [Observation]
      public void should_return_a_clone_of_the_object()
      {
         _result.ShouldBeEqualTo(_clone);
      }
   }
}