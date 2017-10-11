using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using Parameter = PKSim.Core.Snapshots.Parameter;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core
{
   public abstract class concern_for_SnapshotTask : ContextSpecificationAsync<ISnapshotTask>
   {
      protected IDialogCreator _dialogCreator;
      protected IExecutionContext _executionContext;
      protected IParameter _parameter;
      protected string _parameterType = "parameter";
      protected ISnapshotSerializer _snapshotSerializer;
      protected Parameter _parameterSnapshot;
      protected ISnapshotMapper _snapshotMapper;
      protected IObjectTypeResolver _objectTypeResolver;

      protected override Task Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _executionContext = A.Fake<IExecutionContext>();
         _snapshotSerializer = A.Fake<ISnapshotSerializer>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         sut = new SnapshotTask(_dialogCreator, _snapshotSerializer, _snapshotMapper, _executionContext, _objectTypeResolver);

         _parameter = A.Fake<IParameter>();
         _parameter.Name = "Param";
         A.CallTo(() => _objectTypeResolver.TypeFor((IWithName) _parameter)).Returns(_parameterType);

         _parameterSnapshot = new Parameter();
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_parameter)).Returns(_parameterSnapshot);
         return Task.FromResult(true);
      }
   }

   public class When_exporting_a_subject_snapshot_to_file_and_the_user_cancels_the_action : concern_for_SnapshotTask
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(null);
      }

      protected override async Task Because()
      {
         await sut.ExportModelToSnapshot(_parameter);
      }

      [Observation]
      public void should_not_export_the_snapshot_to_file()
      {
         A.CallTo(() => _snapshotSerializer.Serialize(A<object>._, A<string>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_load_the_object_to_export()
      {
         A.CallTo(() => _executionContext.Load(_parameter)).MustNotHaveHappened();
      }
   }

   public class When_exporting_a_subject_snapshot_to_file : concern_for_SnapshotTask
   {
      private readonly string _fileFullPath = "A FILE";
      private string _message;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>()
            .Invokes(x => _message = x.GetArgument<string>(0))
            .Returns(_fileFullPath);
      }

      protected override async Task Because()
      {
         await sut.ExportModelToSnapshot(_parameter);
      }

      [Observation]
      public void should_load_the_object_to_exprot()
      {
         A.CallTo(() => _executionContext.Load(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_ask_the_user_to_select_the_file_where_the_snapshot_will_be_exported_for_the_given_subject()
      {
         _message.Contains(_parameter.Name).ShouldBeTrue();
         _message.Contains(_parameterType).ShouldBeTrue();
      }

      [Observation]
      public void should_retrieve_the_mapper_creating_the_snapshot_object_for_the_subject_to_export()
      {
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_snapshot_to_file()
      {
         A.CallTo(() => _snapshotSerializer.Serialize(_parameterSnapshot, _fileFullPath)).MustHaveHappened();
      }
   }

   public class When_loading_objects_from_snapshot : concern_for_SnapshotTask
   {
      private readonly string _fileName = "FileName";
      private List<Formulation> _formulations;
      private Type _snapshotType;
      private object _snapshot1;
      private object _snapshot2;
      private Formulation _formulation1;
      private Formulation _formulation2;

      protected override async Task Context()
      {
         await base.Context();
         _snapshotType = typeof(Snapshots.Formulation);
         _snapshot1 = new Snapshots.Formulation();
         _snapshot2 = new Snapshots.Formulation();
         _formulation1 = new Formulation();
         _formulation2 = new Formulation();

         A.CallTo(() => _snapshotMapper.SnapshotTypeFor<Formulation>()).Returns(_snapshotType);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileName);
         A.CallTo(() => _snapshotSerializer.DeserializeAsArray(_fileName, _snapshotType)).Returns(new[] {_snapshot1, _snapshot2});
         A.CallTo(() => _snapshotMapper.MapToModel(_snapshot1)).Returns(_formulation1);
         A.CallTo(() => _snapshotMapper.MapToModel(_snapshot2)).Returns(_formulation2);
      }

      protected override async Task Because()
      {
         _formulations = (await sut.LoadModelFromSnapshot<Formulation>()).ToList();
      }

      [Observation]
      public void should_ask_the_user_to_select_the_file_containing_the_snapshot_to_laod()
      {
         A.CallTo(() => _dialogCreator.AskForFileToOpen(A<string>._, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, null, null)).MustHaveHappened();
      }

      [Observation]
      public void should_deserialize_the_file_to_the_matching_snapshot_type_and_return_the_expecting_model_objects()
      {
         _formulations.ShouldContain(_formulation1, _formulation2);
      }
   }

   public class When_loading_objects_from_snapshot_and_the_user_cancels_the_action : concern_for_SnapshotTask
   {
      private List<Formulation> _formulations;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(null);
      }

      protected override async Task Because()
      {
         _formulations = (await sut.LoadModelFromSnapshot<Formulation>()).ToList();
      }

      [Observation]
      public void should_ask_the_user_to_select_the_file_containing_the_snapshot_to_laod()
      {
         A.CallTo(() => _dialogCreator.AskForFileToOpen(A<string>._, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, null, null)).MustHaveHappened();
      }

      [Observation]
      public void should_return_an_empty_enumeration_of_model_being_loaded_from_snapshot()
      {
         _formulations.ShouldBeEmpty();
      }
   }

   public class When_loading_a_project_from_snapshot_file : concern_for_SnapshotTask
   {
      private PKSimProject _project;
      private readonly string _fileName = @"C:\test\SuperProject.json";

      protected override async Task Context()
      {
         await base.Context();
         var snapshotType = typeof(PKSimProject);
         var projectSnapshot = new Project();
         var project = new PKSimProject();

         A.CallTo(() => _snapshotMapper.SnapshotTypeFor<PKSimProject>()).Returns(snapshotType);
         A.CallTo(() => _snapshotSerializer.DeserializeAsArray(_fileName, snapshotType)).Returns(new object[] {projectSnapshot,});
         A.CallTo(() => _snapshotMapper.MapToModel(projectSnapshot)).Returns(project);
      }

      protected override async Task Because()
      {
         _project = await sut.LoadProjectFromSnapshot(_fileName);
      }

      [Observation]
      public void should_return_a_project_with_the_name_set_to_the_name_of_the_file()
      {
         _project.Name.ShouldBeEqualTo("SuperProject");
      }

      [Observation]
      public void should_have_marked_the_project_has_changed()
      {
         _project.HasChanged.ShouldBeTrue();
      }
   }
}