using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportProjectToSnapshotCommand : ContextSpecification<ExportProjectToSnapshotCommand>
   {
      protected IProjectTask _projectTask;

      protected override void Context()
      {
         _projectTask= A.Fake<IProjectTask>();
         sut = new ExportProjectToSnapshotCommand(_projectTask);
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
         A.CallTo(() => _projectTask.ExportCurrentProjectToSnapshot()).MustHaveHappened();
      }
   }
}