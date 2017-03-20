using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_NewIndividualCommand  : ContextSpecification<NewIndividualCommand>
    {
        protected IIndividualTask _individualTask;

        protected override void Context()
        {
            _individualTask = A.Fake<IIndividualTask>();
            sut = new NewIndividualCommand(_individualTask);
        }

    }

    
    public class When_the_create_individual_command_is_being_executed : concern_for_NewIndividualCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverate_the_individual_task_to_create_an_individual()
        {
            A.CallTo(() => _individualTask.AddToProject()).MustHaveHappened();
        }
    }
}