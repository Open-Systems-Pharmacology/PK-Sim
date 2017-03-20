using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_SaveProjectAsCommand : ContextSpecification<SaveProjectAsCommand>
    {
        protected IProjectTask _projectTask;

        protected override void Context()
        {
            _projectTask = A.Fake<IProjectTask>();
            sut = new SaveProjectAsCommand(_projectTask);
        }
    }

    
    public class When_saving_a_project_as : concern_for_SaveProjectAsCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_project_task_to_save_the_project_as()
        {
            A.CallTo(() => _projectTask.SaveCurrentProjectAs()).MustHaveHappened();
        }
    }
}	