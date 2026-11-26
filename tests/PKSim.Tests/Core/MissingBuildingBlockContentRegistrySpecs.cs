using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_MissingBuildingBlockContentRegistry : ContextSpecification<IMissingBuildingBlockContentRegistry>
   {
      protected BuildingBlockWithMissingContent _buildingBlock1;
      protected BuildingBlockWithMissingContent _buildingBlock2;

      protected override void Context()
      {
         sut = new MissingBuildingBlockContentRegistry();
         _buildingBlock1 = new BuildingBlockWithMissingContent {Id = "1"};
         _buildingBlock2 = new BuildingBlockWithMissingContent {Id = "2"};
      }
   }

   public class When_adding_building_blocks_without_content_to_the_registry : concern_for_MissingBuildingBlockContentRegistry
   {
      protected override void Because()
      {
         sut.Add(_buildingBlock1);
         sut.Add(_buildingBlock2);
      }

      [Observation]
      public void should_return_all_added_building_blocks()
      {
         sut.All().ShouldOnlyContainInOrder(_buildingBlock1, _buildingBlock2);
      }

      [Observation]
      public void should_return_a_copy_of_the_registered_building_blocks()
      {
         var all = sut.All();
         sut.Clear();
         all.ShouldOnlyContainInOrder(_buildingBlock1, _buildingBlock2);
      }
   }

   public class When_clearing_the_registry : concern_for_MissingBuildingBlockContentRegistry
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(_buildingBlock1);
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_not_contain_any_building_block()
      {
         sut.All().ShouldBeEmpty();
      }
   }
}
