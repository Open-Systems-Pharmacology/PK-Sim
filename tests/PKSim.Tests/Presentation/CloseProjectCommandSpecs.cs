using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_CloseProjectCommand : ContextSpecification<CloseProjectCommand>
    {
        protected IProjectTask _projectTask;

        protected override void Context()
        {
            _projectTask = A.Fake<IProjectTask>();
            sut = new CloseProjectCommand(_projectTask);
        }
    }


    public class When_closing_a_project : concern_for_CloseProjectCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_project_task_to_close_the_current_project()
        {
            A.CallTo(() => _projectTask.CloseCurrentProject()).MustHaveHappened();
        }
    }
}	