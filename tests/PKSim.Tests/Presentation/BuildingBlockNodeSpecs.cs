using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_UsedBuildingBlockInSimulationNode : ContextSpecification<UsedBuildingBlockInSimulationNode>
   {
      protected IPKSimBuildingBlock _buildingBlock;
      protected Simulation _simulation;
      protected UsedBuildingBlock _usedBuildingBlock;

      protected override void Context()
      {
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _buildingBlock.Id = "tralala";
         _simulation = A.Fake<Simulation>();
         _usedBuildingBlock = new UsedBuildingBlock("Template", PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.Id = "toto";
         _simulation.Id = "trialai";
         A.CallTo(() => _simulation.UsedBuildingBlockById(_usedBuildingBlock.Id)).Returns(_usedBuildingBlock);
         sut = new UsedBuildingBlockInSimulationNode(_simulation, _usedBuildingBlock, _buildingBlock);
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
   }
}