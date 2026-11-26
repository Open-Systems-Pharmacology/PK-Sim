using System.Collections.Generic;
using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectPersistor : ContextSpecification<IProjectPersistor>
   {
      protected IProjectMetaDataToProjectMapper _projectMetaDataToProjectMapper;
      protected IProjectToProjectMetaDataMapper _projectToProjectMetaDataMapper;
      protected ICompressedSerializationManager _serializationManager;
      protected IObjectTypeResolver _objectTypeResolver;
      protected PKSimProject _project;
      protected ProjectMetaData _projectMetaData;
      protected ISession _session;
      protected IList<ProjectMetaData> _listOfProjectsInDatabase;

      protected override void Context()
      {
         _project = A.Fake<PKSimProject>();
         _session = A.Fake<ISession>();
         _projectMetaData = new ProjectMetaData {Id = 1};
         _listOfProjectsInDatabase = new List<ProjectMetaData>();
         _projectToProjectMetaDataMapper = A.Fake<IProjectToProjectMetaDataMapper>();
         _projectMetaDataToProjectMapper = A.Fake<IProjectMetaDataToProjectMapper>();
         _serializationManager = A.Fake<ICompressedSerializationManager>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         A.CallTo(() => _session.BeginTransaction()).Returns(A.Fake<ITransaction>());
         var criteria = A.Fake<ICriteria>();
         A.CallTo(() => _session.CreateCriteria<ProjectMetaData>()).Returns(criteria);
         A.CallTo(() => criteria.List<ProjectMetaData>()).Returns(_listOfProjectsInDatabase);
         sut = new ProjectPersistor(_projectToProjectMetaDataMapper, _projectMetaDataToProjectMapper, _serializationManager, _objectTypeResolver);
      }
   }

   public class When_told_to_save_a_project_that_was_never_saved_before : concern_for_ProjectPersistor
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _projectToProjectMetaDataMapper.MapFrom(_project)).Returns(_projectMetaData);
      }

      protected override void Because()
      {
         sut.Save(_project, _session);
      }

      [Observation]
      public void should_retrieve_the_meta_data_for_that_project()
      {
         A.CallTo(() => _projectToProjectMetaDataMapper.MapFrom(_project)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_meda_date_into_the_session()
      {
         A.CallTo(() => _session.Save(_projectMetaData)).MustHaveHappened();
      }
   }

   public class When_told_to_save_a_project_that_has_been_already_saved : concern_for_ProjectPersistor
   {
      private ProjectMetaData _oldProjectMetaData;

      protected override void Context()
      {
         base.Context();
         _oldProjectMetaData = A.Fake<ProjectMetaData>();
         A.CallTo(() => _projectToProjectMetaDataMapper.MapFrom(_project)).Returns(_projectMetaData);
         _listOfProjectsInDatabase.Add(_oldProjectMetaData);
      }

      protected override void Because()
      {
         sut.Save(_project, _session);
      }

      [Observation]
      public void should_update_the_old_project_with_the_properties_from_the_new_project()
      {
         A.CallTo(() => _oldProjectMetaData.UpdateFrom(_projectMetaData, _session)).MustHaveHappened();
      }

      [Observation]
      public void should_not_call_the_save_method_for_the_session()
      {
         A.CallTo(() => _session.Save(A<object>.Ignored)).MustNotHaveHappened();
      }
   }

   public abstract class concern_for_ProjectPersistor_saving_building_blocks_without_content : concern_for_ProjectPersistor
   {
      protected CompoundMetaData _newCompoundMetaDataWithoutContent;
      protected RandomPopulationMetaData _newPopulationMetaDataWithoutContent;
      protected CompoundMetaData _existingCompoundMetaDataWithoutContent;
      protected Compound _newCompound;
      protected RandomPopulation _newPopulation;
      protected Compound _existingCompound;
      protected ProjectMetaData _oldProjectMetaData;
      protected byte[] _serializedCompound = {1, 2, 3};
      protected byte[] _serializedPopulation = {4, 5, 6};

      protected override void Context()
      {
         base.Context();
         //the building blocks were not flagged as changed, hence the mapper did not serialize their content
         _newCompoundMetaDataWithoutContent = new CompoundMetaData {Id = "NEW_COMPOUND", Name = "New compound"};
         _newPopulationMetaDataWithoutContent = new RandomPopulationMetaData {Id = "NEW_POP", Name = "New population"};
         _existingCompoundMetaDataWithoutContent = new CompoundMetaData {Id = "OLD_COMPOUND", Name = "Old compound"};
         _projectMetaData.AddBuildingBlock(_newCompoundMetaDataWithoutContent);
         _projectMetaData.AddBuildingBlock(_newPopulationMetaDataWithoutContent);
         _projectMetaData.AddBuildingBlock(_existingCompoundMetaDataWithoutContent);

         _newCompound = new Compound {Id = _newCompoundMetaDataWithoutContent.Id, Name = _newCompoundMetaDataWithoutContent.Name, IsLoaded = true, HasChanged = false};
         _newPopulation = new RandomPopulation {Id = _newPopulationMetaDataWithoutContent.Id, Name = _newPopulationMetaDataWithoutContent.Name, IsLoaded = true, HasChanged = false};
         _existingCompound = new Compound {Id = _existingCompoundMetaDataWithoutContent.Id, Name = _existingCompoundMetaDataWithoutContent.Name, IsLoaded = false};
         A.CallTo(() => _project.BuildingBlockById(_newCompound.Id)).Returns(_newCompound);
         A.CallTo(() => _project.BuildingBlockById(_newPopulation.Id)).Returns(_newPopulation);
         A.CallTo(() => _project.BuildingBlockById(_existingCompound.Id)).Returns(_existingCompound);
         A.CallTo(() => _projectToProjectMetaDataMapper.MapFrom(_project)).Returns(_projectMetaData);

         //the persistor serializes the building blocks through their IPKSimBuildingBlock interface, as the mapper does
         A.CallTo(() => _serializationManager.Serialize((IPKSimBuildingBlock) _newCompound, true)).Returns(_serializedCompound);
         A.CallTo(() => _serializationManager.Serialize((IPKSimBuildingBlock) _newPopulation, false)).Returns(_serializedPopulation);

         //only the old compound exists in the database
         _oldProjectMetaData = A.Fake<ProjectMetaData>();
         A.CallTo(() => _oldProjectMetaData.BuildingBlocks).Returns(new List<BuildingBlockMetaData> {new CompoundMetaData {Id = _existingCompound.Id}});
         _listOfProjectsInDatabase.Add(_oldProjectMetaData);
      }
   }

   public class When_told_to_save_a_project_containing_new_loaded_building_blocks_that_are_not_flagged_as_changed : concern_for_ProjectPersistor_saving_building_blocks_without_content
   {
      protected override void Because()
      {
         sut.Save(_project, _session);
      }

      [Observation]
      public void should_serialize_the_content_of_the_new_building_blocks_so_that_they_are_not_written_without_content()
      {
         _newCompoundMetaDataWithoutContent.Content.Data.ShouldBeEqualTo(_serializedCompound);
      }

      [Observation]
      public void should_not_compress_the_content_of_a_population()
      {
         _newPopulationMetaDataWithoutContent.Content.Data.ShouldBeEqualTo(_serializedPopulation);
      }

      [Observation]
      public void should_not_serialize_the_content_of_a_building_block_that_already_exists_in_the_database()
      {
         _existingCompoundMetaDataWithoutContent.Content.Data.ShouldBeNull();
         A.CallTo(() => _serializationManager.Serialize((IPKSimBuildingBlock) _existingCompound, A<bool>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_update_the_project_in_the_database()
      {
         A.CallTo(() => _oldProjectMetaData.UpdateFrom(_projectMetaData, _session)).MustHaveHappened();
      }
   }

   public class When_told_to_save_a_project_containing_a_new_building_block_that_is_not_loaded : concern_for_ProjectPersistor_saving_building_blocks_without_content
   {
      protected override void Context()
      {
         base.Context();
         _newCompound.IsLoaded = false;
         A.CallTo(() => _objectTypeResolver.TypeFor((IPKSimBuildingBlock) _newCompound)).Returns(PKSimConstants.ObjectTypes.Compound);
      }

      [Observation]
      public void should_refuse_to_save_the_project()
      {
         The.Action(() => sut.Save(_project, _session)).ShouldThrowAn<PKSimException>();
      }

      [Observation]
      public void should_name_the_building_block_without_content_in_the_exception_message()
      {
         var exception = catchException();
         exception.ShouldNotBeNull();
         exception.Message.ShouldBeEqualTo(PKSimConstants.Error.CannotSaveBuildingBlockWithoutContent(_newCompound.Name, PKSimConstants.ObjectTypes.Compound));
      }

      [Observation]
      public void should_not_write_anything_into_the_database()
      {
         catchException();
         A.CallTo(() => _oldProjectMetaData.UpdateFrom(A<ProjectMetaData>._, A<ISession>._)).MustNotHaveHappened();
         A.CallTo(() => _session.Save(A<object>._)).MustNotHaveHappened();
      }

      private PKSimException catchException()
      {
         try
         {
            sut.Save(_project, _session);
            return null;
         }
         catch (PKSimException e)
         {
            return e;
         }
      }
   }

   public class When_told_to_save_a_project_for_the_first_time_containing_a_building_block_that_is_not_flagged_as_changed : concern_for_ProjectPersistor_saving_building_blocks_without_content
   {
      protected override void Context()
      {
         base.Context();
         //no project in database: all building blocks are new
         _listOfProjectsInDatabase.Clear();
         A.CallTo(() => _serializationManager.Serialize((IPKSimBuildingBlock) _existingCompound, true)).Returns(_serializedCompound);
         _existingCompound.IsLoaded = true;
      }

      protected override void Because()
      {
         sut.Save(_project, _session);
      }

      [Observation]
      public void should_serialize_the_content_of_all_building_blocks_without_content()
      {
         _newCompoundMetaDataWithoutContent.Content.Data.ShouldBeEqualTo(_serializedCompound);
         _newPopulationMetaDataWithoutContent.Content.Data.ShouldBeEqualTo(_serializedPopulation);
         _existingCompoundMetaDataWithoutContent.Content.Data.ShouldBeEqualTo(_serializedCompound);
      }

      [Observation]
      public void should_save_the_project_in_the_session()
      {
         A.CallTo(() => _session.Save(_projectMetaData)).MustHaveHappened();
      }
   }

   public class When_loading_a_project_that_was_created_with_an_newer_version_from_the_application : concern_for_ProjectPersistor
   {
      protected override void Context()
      {
         base.Context();
         _projectMetaData.Version = ProjectVersions.Current + 1;
         _listOfProjectsInDatabase.Add(_projectMetaData);
         A.CallTo(() => _projectMetaDataToProjectMapper.MapFrom(_projectMetaData)).Returns(_project);
      }

      [Observation]
      public void should_throw_an_invalid_project_version_exception()
      {
         The.Action(() => sut.Load(_session)).ShouldThrowAn<InvalidProjectVersionException>();
      }
   }

   public class When_loading_a_project_that_was_created_with_an_version_of_the_application_that_is_not_supported_anymore : concern_for_ProjectPersistor
   {
      protected override void Context()
      {
         base.Context();
         _projectMetaData.Version = ProjectVersions.UNSUPPORTED;
         _listOfProjectsInDatabase.Add(_projectMetaData);
         A.CallTo(() => _projectMetaDataToProjectMapper.MapFrom(_projectMetaData)).Returns(_project);
      }

      [Observation]
      public void should_throw_an_invalid_project_version_exception()
      {
         The.Action(() => sut.Load(_session)).ShouldThrowAn<InvalidProjectVersionException>();
      }
   }

   public class When_loading_a_project_from_a_corrupted_file : concern_for_ProjectPersistor
   {
      [Observation]
      public void should_throw_an_invalid_project_file_exception()
      {
         The.Action(() => sut.Load(_session)).ShouldThrowAn<InvalidProjectFileException>();
      }
   }
}
