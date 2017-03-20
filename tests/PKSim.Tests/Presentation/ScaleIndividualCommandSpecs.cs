using PKSim.Core.Model;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_ScaleIndividualCommand : ContextSpecification<ScaleIndividualCommand>
    {
        protected IIndividualTask _individualTask;
        protected PKSim.Core.Model.Individual _individual;

        protected override void Context()
        {
            _individualTask = A.Fake<IIndividualTask>();
            _individual = A.Fake<PKSim.Core.Model.Individual>();
            sut = new ScaleIndividualCommand(_individualTask);
            sut.For(_individual);
        }
            
    }

    
    public class When_told_to_execute : concern_for_ScaleIndividualCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_individual_task_that_will_scale_the_given_individual()
        {
            A.CallTo(() => _individualTask.ScaleIndividual(_individual)).MustHaveHappened();
        }
    }
}