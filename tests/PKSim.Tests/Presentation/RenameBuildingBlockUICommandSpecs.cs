using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameBuildingBlockUICommand : ContextSpecification<RenameBuildingBlockUICommand>
   {
      protected IBuildingBlockTask _buildingBlockTask;

      protected override void Context()
      {
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         sut = new RenameBuildingBlockUICommand(_buildingBlockTask);
      }
   }

   
   public class When_renaming_a_building_block : concern_for_RenameBuildingBlockUICommand
   {
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         sut.For(_buildingBlock);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_buildong_block_task_to_rename_the_entity()
      {
         A.CallTo(() => _buildingBlockTask.Rename(_buildingBlock)).MustHaveHappened();
      }
   }
}