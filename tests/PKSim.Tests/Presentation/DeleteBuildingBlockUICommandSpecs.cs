using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
    public abstract class concern_for_DeleteBuildingBlockUICommand : ContextSpecification<DeleteBuildingBlockUICommand>
    {
        protected IBuildingBlockTask _buildingBlockTask;
        protected IPKSimBuildingBlock _buildingBlockToDelete;

        protected override void Context()
        {
            _buildingBlockTask = A.Fake<IBuildingBlockTask>();
            _buildingBlockToDelete = A.Fake<IPKSimBuildingBlock>();
            sut = new DeleteBuildingBlockUICommand(_buildingBlockTask);
            sut.For(_buildingBlockToDelete);
        }
    }

    
    public class When_the_delete_building_block_ui_command_is_being_executed : concern_for_DeleteBuildingBlockUICommand
    {
        protected override void Because()
        {
            sut.Execute();
        }

        [Observation]
        public void should_leverate_the_individual_task_to_delete_the_individual()
        {
            A.CallTo(() => _buildingBlockTask.Delete(_buildingBlockToDelete)).MustHaveHappened();
        }
    }
}