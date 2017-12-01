using System.Collections.Generic;
using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectPersistor : ContextSpecification<IProjectPersistor>
   {
      protected IProjectMetaDataToProjectMapper _projectMetaDataToProjectMapper;
      protected IProjectToProjectMetaDataMapper _projectToProjectMetaDataMapper;
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
         A.CallTo(() => _session.BeginTransaction()).Returns(A.Fake<ITransaction>());
         var criteria = A.Fake<ICriteria>();
         A.CallTo(() => _session.CreateCriteria<ProjectMetaData>()).Returns(criteria);
         A.CallTo(() => criteria.List<ProjectMetaData>()).Returns(_listOfProjectsInDatabase);
         sut = new ProjectPersistor(_projectToProjectMetaDataMapper, _projectMetaDataToProjectMapper);
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

   public class When_loading_a_project_that_was_created_with_an_older_version_from_the_application : concern_for_ProjectPersistor
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

   public class When_loading_a_project_from_a_corrupted_file : concern_for_ProjectPersistor
   {
      [Observation]
      public void should_throw_an_invalid_project_file_exception()
      {
         The.Action(() => sut.Load(_session)).ShouldThrowAn<InvalidProjectFileException>();
      }
   }
}