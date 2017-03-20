using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_NewProjectCommand : ContextSpecification<NewProjectCommand>
    {
        protected IProjectTask _projectTask;

        protected override void Context()
        {
            _projectTask = A.Fake<IProjectTask>();
            sut = new NewProjectCommand(_projectTask);
        }
    }

    
    public class When_creating_a_new_project : concern_for_NewProjectCommand
    {
        [Observation]
        public void should_leverage_the_project_task_to_create_a_new_project()
        {
            A.CallTo(() => _projectTask.NewProject()).MustHaveHappened();
        }

        protected override void Because()
        {
            sut.Execute();
        }
    }
}