using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_LoadProjectFromSnapshotCommand : ContextSpecification<LoadProjectFromSnapshotCommand>
   {
      protected IProjectTask _projectTask;

      protected override void Context()
      {
         _projectTask = A.Fake<IProjectTask>();
         sut = new LoadProjectFromSnapshotCommand(_projectTask);
      }
   }

   public class When_executing_the_load_project_from_snapshot_command : concern_for_LoadProjectFromSnapshotCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_project_task_to_load_the_project_snapshot_in_the_current_project()
      {
         A.CallTo(() => _projectTask.LoadProjectFromSnapshot()).MustHaveHappened();
      }
   }
}