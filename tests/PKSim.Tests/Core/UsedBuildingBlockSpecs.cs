using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_used_building_block : ContextSpecification<UsedBuildingBlock>
   {
      protected int _version;
      protected string _id;

      protected override void Context()
      {
         _id = "talal";
         sut = new UsedBuildingBlock("toto", PKSimBuildingBlockType.Formulation) {Version = _version};
      }
   }

   
   public class When_setting_the_building_block_used : concern_for_used_building_block
   {
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Id = _id;
      }

      [Observation]
      public void should_update_the_id()
      {
         sut.Id.ShouldNotBeEqualTo(_id);
      }
   }
}