using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_OpenProjectCommand : ContextSpecification<OpenProjectCommand>
    {
        protected IProjectTask _projectTask;

        protected override void Context()
        {
            _projectTask = A.Fake<IProjectTask>();
            sut = new OpenProjectCommand(_projectTask);
        }
    }

    
    public class When_opening_a_project : concern_for_OpenProjectCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_project_task_to_open_the_project()
        {
            A.CallTo(() => _projectTask.OpenProject()).MustHaveHappened();
        }
    }
}	