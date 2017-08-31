using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_LoadBuildingBlockFromSnapshotUICommand : ContextSpecification<LoadBuildingBlockFromSnapshotUICommand<Formulation>>
   {
      protected IBuildingBlockTask<Formulation> _buildingBlockTask;

      protected override void Context()
      {
         _buildingBlockTask = A.Fake<IBuildingBlockTask<Formulation>>();
         sut = new LoadBuildingBlockFromSnapshotUICommand<Formulation>(_buildingBlockTask);
      }
   }

   public class When_executing_the_load_building_block_from_snaphsot_ui_command : concern_for_LoadBuildingBlockFromSnapshotUICommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_building_block_task_to_load_from_snapshot()
      {
         A.CallTo(() => _buildingBlockTask.LoadFromSnapshot()).MustHaveHappened();
      }
   }
}