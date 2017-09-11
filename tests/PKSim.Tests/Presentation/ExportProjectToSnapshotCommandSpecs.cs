using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportProjectToSnapshotCommand : ContextSpecification<ExportProjectToSnapshotCommand>
   {
      protected ISnapshotTask _snapshotTask;
      protected IPKSimProjectRetriever _projectRetriever;

      protected override void Context()
      {
         _snapshotTask = A.Fake<ISnapshotTask>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         sut = new ExportProjectToSnapshotCommand(_snapshotTask, _projectRetriever);
      }
   }

   public class When_executing_the_export_project_to_snapshot_command : concern_for_ExportProjectToSnapshotCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_snapshot_task_to_export_the_project_to_snapshot()
      {
         A.CallTo(() => _snapshotTask.ExportSnapshot(_projectRetriever.Current)).MustHaveHappened();
      }
   }
}