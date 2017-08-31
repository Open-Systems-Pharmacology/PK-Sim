using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_SnapshotTask : ContextSpecification<ISnapshotTask>
   {
      protected IDialogCreator _dialogCreator;
      protected IExecutionContext _executionContext;
      protected IParameter _parameter;
      protected string _parameterType = "parameter";
      protected ISnapshotSerializer _snapshotSerializer;
      protected Parameter _parameterSnapshot;
      protected ISnapshotMapper _snapshotMapper;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _executionContext = A.Fake<IExecutionContext>();
         _snapshotSerializer = A.Fake<ISnapshotSerializer>();
         _snapshotMapper= A.Fake<ISnapshotMapper>();
         sut = new SnapshotTask(_dialogCreator, _snapshotSerializer, _snapshotMapper, _executionContext);

         _parameter = A.Fake<IParameter>();
         _parameter.Name = "Param";
         A.CallTo(() => _executionContext.TypeFor(_parameter)).Returns(_parameterType);

         _parameterSnapshot = new Parameter();
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_parameter)).Returns(_parameterSnapshot);
      }
   }

   public class When_exporting_a_subject_snapshot_to_file_and_the_user_cancels_the_action : concern_for_SnapshotTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(null);
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

      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>()
            .Invokes(x => _message = x.GetArgument<string>(0))
            .Returns(_fileFullPath);
      }

      protected override void Because()
      {
         sut.ExportSnapshot(_parameter);
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
}