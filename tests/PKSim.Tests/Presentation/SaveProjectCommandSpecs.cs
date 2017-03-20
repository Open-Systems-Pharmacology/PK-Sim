using OSPSuite.BDDHelper;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
    public abstract class concern_for_SaveProjectCommand : ContextSpecification<SaveProjectCommand>
    {
        protected IProjectTask _projectTask;

        protected override void Context()
        {
            _projectTask = A.Fake<IProjectTask>();
            sut = new SaveProjectCommand(_projectTask);
        }
    }

    
    public class When_saving_a_project : concern_for_SaveProjectCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_project_task_to_save_the_project()
        {
            A.CallTo(() => _projectTask.SaveCurrentProject()).MustHaveHappened();
        }
    }
}