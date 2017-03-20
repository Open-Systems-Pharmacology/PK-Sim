using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockChangeCommandExtensions : StaticContextSpecification
   {
      protected IBuildingBlockChangeCommand _buildingBlockChangeCommand;
      protected IBuildingBlockChangeCommand _inverseBuildingBlockChangeCommand;

      protected override void Context()
      {
         _buildingBlockChangeCommand = A.Fake<IBuildingBlockChangeCommand>();
         _inverseBuildingBlockChangeCommand = A.Fake<IBuildingBlockChangeCommand>();
      }
   }

   
   public class When_marking_an_building_block_command_as_the_inverse_from_another_command : concern_for_BuildingBlockChangeCommandExtensions
   {
      protected override void Context()
      {
         base.Context();
         _buildingBlockChangeCommand.Id = A.Fake<CommandId>();
         _buildingBlockChangeCommand.IncrementVersion = true;
      }

      protected override void Because()
      {
         _inverseBuildingBlockChangeCommand.AsInverseFor(_buildingBlockChangeCommand);
      }

      [Observation]
      public void should_set_the_increment_version_flag_to_the_oposite_value_from_the_source_command()
      {
         _inverseBuildingBlockChangeCommand.IncrementVersion.ShouldBeFalse();
      }
   }
}