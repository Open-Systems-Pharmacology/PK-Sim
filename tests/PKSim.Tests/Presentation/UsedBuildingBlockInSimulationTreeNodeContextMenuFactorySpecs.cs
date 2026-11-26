using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.ContextMenus;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation
{
   public abstract class concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory : ContextSpecification<UsedIndividualInSimulationTreeNodeContextMenuFactory>
   {
      protected IBuildingBlockRetriever _buildingBlockRetriever;
      protected IPresenterWithContextMenu<ITreeNode> _presenter;
      protected Simulation _simulation;
      protected UsedBuildingBlock _usedIndividual;
      protected Individual _templateIndividual;
      protected UsedBuildingBlockInSimulationNode _usedIndividualNode;

      protected override void Context()
      {
         _buildingBlockRetriever = A.Fake<IBuildingBlockRetriever>();
         _presenter = A.Fake<IPresenterWithContextMenu<ITreeNode>>();
         _templateIndividual = new Individual().WithId("templateIndividual");
         _usedIndividual = new UsedBuildingBlock(_templateIndividual.Id, PKSimBuildingBlockType.Individual) {Id = "usedIndividual"};
         _simulation = new IndividualSimulation();
         _simulation.AddUsedBuildingBlock(_usedIndividual);
         _usedIndividualNode = new UsedBuildingBlockInSimulationNode(_simulation, _usedIndividual, _buildingBlockRetriever);
         sut = new UsedIndividualInSimulationTreeNodeContextMenuFactory(A.Fake<IContainer>());
      }
   }

   public class When_checking_if_the_used_individual_context_menu_factory_is_satisfied_by_a_used_individual_node : concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_templateIndividual.Id)).Returns(_templateIndividual);
      }

      [Observation]
      public void should_return_true() => sut.IsSatisfiedBy(_usedIndividualNode, _presenter).ShouldBeTrue();
   }

   public class When_checking_if_the_used_individual_context_menu_factory_is_satisfied_by_a_used_individual_node_created_while_the_template_individual_was_being_loaded : concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory
   {
      protected override void Context()
      {
         base.Context();
         //the template individual is not registered while it is being lazy loaded (e.g. project opened from the file menu)
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_templateIndividual.Id)).Returns(null);
         _usedIndividualNode = new UsedBuildingBlockInSimulationNode(_simulation, _usedIndividual, _buildingBlockRetriever);
      }

      //loading finished, the template individual is registered again
      protected override void Because() => A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_templateIndividual.Id)).Returns(_templateIndividual);

      [Observation]
      public void should_return_true() => sut.IsSatisfiedBy(_usedIndividualNode, _presenter).ShouldBeTrue();
   }

   public class When_checking_if_the_used_individual_context_menu_factory_is_satisfied_by_a_used_individual_node_whose_template_does_not_exist : concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_templateIndividual.Id)).Returns(null);
      }

      [Observation]
      public void should_return_false() => sut.IsSatisfiedBy(_usedIndividualNode, _presenter).ShouldBeFalse();
   }

   public class When_checking_if_the_used_individual_context_menu_factory_is_satisfied_by_a_used_building_block_node_whose_template_is_not_an_individual : concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_templateIndividual.Id)).Returns(new Compound().WithId(_templateIndividual.Id));
      }

      [Observation]
      public void should_return_false() => sut.IsSatisfiedBy(_usedIndividualNode, _presenter).ShouldBeFalse();
   }

   public class When_checking_if_the_used_individual_context_menu_factory_is_satisfied_by_a_node_that_is_not_a_used_building_block_node : concern_for_UsedIndividualInSimulationTreeNodeContextMenuFactory
   {
      [Observation]
      public void should_return_false() => sut.IsSatisfiedBy(A.Fake<ITreeNode>(), _presenter).ShouldBeFalse();
   }
}
