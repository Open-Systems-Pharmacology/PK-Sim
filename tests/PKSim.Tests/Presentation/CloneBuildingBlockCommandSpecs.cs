using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
    public abstract class concern_for_CloneBuildingBlockCommand : ContextSpecification<CloneBuildingBlockCommand<IPKSimBuildingBlock>>
    {
        protected IPKSimBuildingBlock _entityToClone;
        protected IBuildingBlockTask _buildingBlockTask;

        protected override void Context()
        {
            _entityToClone =A.Fake<IPKSimBuildingBlock>();
            _buildingBlockTask = A.Fake<IBuildingBlockTask>();
            sut = new CloneBuildingBlockCommand<IPKSimBuildingBlock>(_buildingBlockTask);
            sut.For(_entityToClone);
        }
    }

    
    public class When_the_clone_ui_command_is_executing : concern_for_CloneBuildingBlockCommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverage_the_entity_task_to_clone_the_entity_it_was_initialized_with()
        {
            A.CallTo(() => _buildingBlockTask.Clone(_entityToClone)).MustHaveHappened();
        }
    }
}	