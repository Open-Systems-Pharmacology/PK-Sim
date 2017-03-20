using OSPSuite.BDDHelper;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_OpenMRUProjectCommand : ContextSpecification<OpenMRUProjectCommand>
   {
      protected IProjectTask _projectTask;
      protected string _projectPath;

      protected override void Context()
      {
         _projectTask = A.Fake<IProjectTask>();
         _projectPath = "tralalal";
         sut = new OpenMRUProjectCommand(_projectTask);
         sut.ProjectPath = _projectPath;
      }
   }

   
   public class When_opening_a_project_from_a_project_file_path_that_was_most_recently_used : concern_for_OpenMRUProjectCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_project_task_to_open_the_project_for_the_given_file()
      {
         A.CallTo(() => _projectTask.OpenProjectFrom(_projectPath)).MustHaveHappened();
      }
   }
}