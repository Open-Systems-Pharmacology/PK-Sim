using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportSnapshotCommand : ContextSpecification<ExportSnapshotUICommand<IParameter>>
   {
      protected ISnapshotTask _snapshotTask;
      protected IParameter _parameter;

      protected override void Context()
      {
         _parameter= A.Fake<IParameter>();
         _snapshotTask= A.Fake<ISnapshotTask>();
         sut = new ExportSnapshotUICommand<IParameter>(_snapshotTask) {Subject = _parameter};
      }
   }

   public class When_executing_the_export_snapshot_command : concern_for_ExportSnapshotCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_export_the_snapshot_for_the_given_subject()
      {
         A.CallTo(() => _snapshotTask.ExportModelToSnapshot(_parameter)).MustHaveHappened();
      }
   }
}	