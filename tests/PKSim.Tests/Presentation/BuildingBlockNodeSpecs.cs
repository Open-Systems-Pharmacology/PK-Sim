using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_UsedBuildingBlockInSimulationNode : ContextSpecification<UsedBuildingBlockInSimulationNode>
   {
      protected IPKSimBuildingBlock _buildingBlock;
      protected Simulation _simulation;
      protected UsedBuildingBlock _usedBuildingBlock;
      protected IBuildingBlockRetriever _buildingBlockRetriever;

      protected override void Context()
      {
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Id = "tralala";
         _simulation = A.Fake<Simulation>();
         _buildingBlockRetriever = A.Fake<IBuildingBlockRetriever>();
         _usedBuildingBlock = new UsedBuildingBlock(_buildingBlock.Id, PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.Id = "toto";
         _simulation.Id = "trialai";
         A.CallTo(() => _simulation.UsedBuildingBlockById(_usedBuildingBlock.Id)).Returns(_usedBuildingBlock);
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_buildingBlock.Id)).Returns(_buildingBlock);
         sut = new UsedBuildingBlockInSimulationNode(_simulation, _usedBuildingBlock, _buildingBlockRetriever);
      }
   }

   public class When_asked_for_the_properties_it_was_created_with : concern_for_UsedBuildingBlockInSimulationNode
   {
      [Observation]
      public void should_return_the_properties()
      {
         sut.Simulation.ShouldBeEqualTo(_simulation);
         sut.UsedBuildingBlock.ShouldBeEqualTo(_usedBuildingBlock);
      }

      [Observation]
      public void should_return_the_template_building_block_with_the_template_id_of_the_used_building_block() => sut.TemplateBuildingBlock.ShouldBeEqualTo(_buildingBlock);
   }

   public class When_asked_for_the_template_building_block_of_a_node_created_while_the_template_was_not_available : concern_for_UsedBuildingBlockInSimulationNode
   {
      protected override void Context()
      {
         base.Context();
         //template not registered (e.g. being lazy loaded) when the node is created
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_buildingBlock.Id)).Returns(null);
         sut = new UsedBuildingBlockInSimulationNode(_simulation, _usedBuildingBlock, _buildingBlockRetriever);
      }

      [Observation]
      public void should_return_null_as_long_as_the_template_is_not_available() => sut.TemplateBuildingBlock.ShouldBeNull();

      [Observation]
      public void should_return_the_template_building_block_once_it_is_available()
      {
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_buildingBlock.Id)).Returns(_buildingBlock);
         sut.TemplateBuildingBlock.ShouldBeEqualTo(_buildingBlock);
      }
   }

   public class When_asked_for_the_template_building_block_after_the_template_was_swapped : concern_for_UsedBuildingBlockInSimulationNode
   {
      private IPKSimBuildingBlock _newBuildingBlock;

      protected override void Context()
      {
         base.Context();
         //a swapped building block (e.g. protocol mode change) replaces the template under the same id
         _newBuildingBlock = A.Fake<IPKSimBuildingBlock>().WithId(_buildingBlock.Id);
      }

      protected override void Because() => A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_buildingBlock.Id)).Returns(_newBuildingBlock);

      [Observation]
      public void should_return_the_new_template_building_block() => sut.TemplateBuildingBlock.ShouldBeEqualTo(_newBuildingBlock);
   }

   public class When_asked_for_the_template_building_block_of_a_used_building_block_that_is_not_in_the_simulation_anymore : concern_for_UsedBuildingBlockInSimulationNode
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulation.UsedBuildingBlockById(_usedBuildingBlock.Id)).Returns(null);
      }

      [Observation]
      public void should_return_null() => sut.TemplateBuildingBlock.ShouldBeNull();

      [Observation]
      public void should_not_try_to_retrieve_a_building_block() => A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(A<string>._)).MustNotHaveHappened();
   }
}
