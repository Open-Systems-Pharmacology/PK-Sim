using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockVersionUpdater : ContextSpecification<IBuildingBlockVersionUpdater>
   {
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;

      protected override void Context()
      {
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         sut = new BuildingBlockVersionUpdater(_buildingBlockInSimulationManager);
      }
   }

   
   public class When_updating_the_building_block_version_for_a_decrementing_building_block_change_command : concern_for_BuildingBlockVersionUpdater
   {
      private IPKSimBuildingBlock _buildingBlock;
      private int _version;
      private string _buildingBlockId;
      private IBuildingBlockChangeCommand _buildingBlockChangeCommand;

      protected override void Context()
      {
         base.Context();
         _version = 5;
         _buildingBlockChangeCommand = A.Fake<IBuildingBlockChangeCommand>();
         A.CallTo(() => _buildingBlockChangeCommand.ShouldChangeVersion).Returns(true);
         A.CallTo(() => _buildingBlockChangeCommand.BuildingBlockId).Returns(_buildingBlockId);
         _buildingBlockId = "tralala";
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Version = _version;
      }

      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_buildingBlockChangeCommand, _buildingBlock);
      }

      [Observation]
      public void should_retrieve_the_building_block_involved_in_the_command_change_and_decrement_its_version()
      {
         _buildingBlock.Version.ShouldBeEqualTo(_version - 1);
      }

      [Observation]
      public void should_notify_all_simulation_using_the_building_block_that_their_status_might_have_changed()
      {
         A.CallTo(() => _buildingBlockInSimulationManager.UpdateStatusForSimulationUsing(_buildingBlock)).MustHaveHappened();
      }
   }

   
   public class When_updating_the_building_block_version_for_an_increment_building_block_change_command : concern_for_BuildingBlockVersionUpdater
   {
      private IPKSimBuildingBlock _buildingBlock;
      private int _version;
      private string _buildingBlockId;
      private IBuildingBlockChangeCommand _buildingBlockChangeCommand;

      protected override void Context()
      {
         base.Context();
         _version = 5;
         _buildingBlockChangeCommand = A.Fake<IBuildingBlockChangeCommand>();
         A.CallTo(() => _buildingBlockChangeCommand.IncrementVersion).Returns(true);
         A.CallTo(() => _buildingBlockChangeCommand.BuildingBlockId).Returns(_buildingBlockId);
         A.CallTo(() => _buildingBlockChangeCommand.ShouldChangeVersion).Returns(true);
         _buildingBlockId = "tralala";
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Version = _version;
      }

      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_buildingBlockChangeCommand, _buildingBlock);
      }

      [Observation]
      public void should_retrieve_the_building_block_involved_in_the_command_change_and_increment_its_version()
      {
         _buildingBlock.Version.ShouldBeEqualTo(_version + 1);
      }

      [Observation]
      public void should_notify_all_simulation_using_the_building_block_that_their_status_might_have_changed()
      {
         A.CallTo(() => _buildingBlockInSimulationManager.UpdateStatusForSimulationUsing(_buildingBlock)).MustHaveHappened();
      }
   }

   
   public class When_updating_the_building_block_version_for_an_increment_change_command_that_is_also_a_structure_change_command : concern_for_BuildingBlockVersionUpdater
   {
      private IPKSimBuildingBlock _buidlingBlock;
      private IBuildingBlockChangeCommand _buildingBlockStructureChangeCommand;

      protected override void Context()
      {
         base.Context();
         _buildingBlockStructureChangeCommand = A.Fake<IBuildingBlockStructureChangeCommand>();
         A.CallTo(() => _buildingBlockStructureChangeCommand.ShouldChangeVersion).Returns(true);
         A.CallTo(() => _buildingBlockStructureChangeCommand.IncrementVersion).Returns(true);
         _buidlingBlock = A.Fake<IPKSimBuildingBlock>();
         _buidlingBlock.Version = 8;
         _buidlingBlock.StructureVersion = 5;
      }
      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_buildingBlockStructureChangeCommand,_buidlingBlock);
      }

      [Observation]
      public void should_increase_the_structure_version_of_the_building_block()
      {
         _buidlingBlock.StructureVersion.ShouldBeEqualTo(6);
      }

      [Observation]
      public void should_increase_the_version_of_the_building_block()
      {
         _buidlingBlock.Version.ShouldBeEqualTo(9);
      }
   }

   
   public class When_updating_the_building_block_version_for_a_decrement_change_command_that_is_also_a_structure_change_command : concern_for_BuildingBlockVersionUpdater
   {
      private IPKSimBuildingBlock _buidlingBlock;
      private IBuildingBlockChangeCommand _buildingBlockStructureChangeCommand;

      protected override void Context()
      {
         base.Context();
         _buildingBlockStructureChangeCommand = A.Fake<IBuildingBlockStructureChangeCommand>();
         A.CallTo(() => _buildingBlockStructureChangeCommand.ShouldChangeVersion).Returns(true);
         A.CallTo(() => _buildingBlockStructureChangeCommand.IncrementVersion).Returns(false);
         _buidlingBlock = A.Fake<IPKSimBuildingBlock>();
         _buidlingBlock.Version = 8;
         _buidlingBlock.StructureVersion = 5;
      }
      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_buildingBlockStructureChangeCommand, _buidlingBlock);
      }

      [Observation]
      public void should_decrease_the_structure_version_of_the_building_block()
      {
         _buidlingBlock.StructureVersion.ShouldBeEqualTo(4);
      }

      [Observation]
      public void should_decrease_the_version_of_the_building_block()
      {
         _buidlingBlock.Version.ShouldBeEqualTo(7);
      }
   }

   
   public class When_updating_the_building_block_version_building_block_change_command_that_should_not_change_the_version : concern_for_BuildingBlockVersionUpdater
   {
      private IPKSimBuildingBlock _buildingBlock;
      private int _version;
      private string _buildingBlockId;
      private IBuildingBlockChangeCommand _buildingBlockChangeCommand;

      protected override void Context()
      {
         base.Context();
         _version = 5;
         _buildingBlockChangeCommand = A.Fake<IBuildingBlockChangeCommand>();
         A.CallTo(() => _buildingBlockChangeCommand.ShouldChangeVersion).Returns(false);
         A.CallTo(() => _buildingBlockChangeCommand.BuildingBlockId).Returns(_buildingBlockId);
         _buildingBlockId = "tralala";
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Version = _version;
      }

      protected override void Because()
      {
         sut.UpdateBuildingBlockVersion(_buildingBlockChangeCommand, _buildingBlock);
      }

      [Observation]
      public void should_not_change_the_buildding_block_version_()
      {
         _buildingBlock.Version.ShouldBeEqualTo(_version);
      }

      [Observation]
      public void should_not_notify_any_change_for_the_underlying_simulation()
      {
         A.CallTo(() => _buildingBlockInSimulationManager.UpdateStatusForSimulationUsing(_buildingBlock)).MustNotHaveHappened();
      }
   }
}