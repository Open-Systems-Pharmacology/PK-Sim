using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ContextMenus;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.UICommands;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation
{
   public abstract class concern_for_MultipleUsedBuildingBlockNodeContextMenuFactory : ContextSpecificationWithLocalContainer<MultipleUsedBuildingBlockNodeContextMenuFactory>
   {
      protected IExecutionContext _executionContext;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected List<ITreeNode> _usedBuildingBlocksTreeNode;
      protected IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> _presenter;
      protected UsedBuildingBlock _usedIndividual1;
      protected UsedBuildingBlock _usedIndividual2;
      protected IPKSimBuildingBlock _buildingBlock1;
      protected IPKSimBuildingBlock _buildingBlock2;
      protected UsedBuildingBlock _usedCompound1;
      private IPKSimBuildingBlock _buildingBlock3;
      protected CompareObjectsUICommand _command;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _simulationBuildingBlockUpdater= A.Fake<ISimulationBuildingBlockUpdater>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _container = A.Fake<IContainer>();
         sut = new MultipleUsedBuildingBlockNodeContextMenuFactory(_buildingBlockInProjectManager, _executionContext, _container);

         _usedBuildingBlocksTreeNode = new List<ITreeNode>();
         _presenter = A.Fake<IPresenterWithContextMenu<IReadOnlyList<ITreeNode>>>();

         _buildingBlock1 = A.Fake<IPKSimBuildingBlock>().WithId("bb1");
         _buildingBlock2 = A.Fake<IPKSimBuildingBlock>().WithId("bb2");
         _buildingBlock3 = A.Fake<IPKSimBuildingBlock>().WithId("bb3");

         _usedIndividual1 = new UsedBuildingBlock("1", PKSimBuildingBlockType.Individual) {BuildingBlock = _buildingBlock1};
         _usedIndividual2 = new UsedBuildingBlock("2", PKSimBuildingBlockType.Individual) {BuildingBlock = _buildingBlock2};
         _usedCompound1 = new UsedBuildingBlock("3", PKSimBuildingBlockType.Compound) {BuildingBlock = _buildingBlock3};
         _command = new CompareObjectsUICommand(A.Fake<IEventPublisher>());
         A.CallTo(() => _container.Resolve<CompareObjectsUICommand>()).Returns(_command);
         A.CallTo(() => _executionContext.Resolve<ISimulationBuildingBlockUpdater>()).Returns(_simulationBuildingBlockUpdater);

      }
   }

   public class When_creating_a_context_menu_for_a_set_of_used_building_blocks : concern_for_MultipleUsedBuildingBlockNodeContextMenuFactory
   {
      private Simulation _simulation1;
      private Simulation _simulation2;

      protected override void Context()
      {
         base.Context();
         var usedBuildingBlockNode1 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => usedBuildingBlockNode1.TagAsObject).Returns(_usedIndividual1);
         var usedBuildingBlockNode2 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => usedBuildingBlockNode2.TagAsObject).Returns(_usedIndividual2);
         _usedBuildingBlocksTreeNode.Add(usedBuildingBlockNode1);
         _usedBuildingBlocksTreeNode.Add(usedBuildingBlockNode2);

         _simulation1 = new IndividualSimulation().WithName("S1");
         _simulation2 = new IndividualSimulation().WithName("S2");

         _simulation1.AddUsedBuildingBlock(_usedIndividual1);
         _simulation2.AddUsedBuildingBlock(_usedIndividual2);
         A.CallTo(() => _buildingBlockInProjectManager.SimulationUsing(_usedIndividual1)).Returns(_simulation1);
         A.CallTo(() => _buildingBlockInProjectManager.SimulationUsing(_usedIndividual2)).Returns(_simulation2);

         A.CallTo(() => _simulationBuildingBlockUpdater.BuildingBlockSupportsQuickUpdate(_buildingBlock1)).Returns(true);

      }

      protected override void Because()
      {
         sut.CreateFor(_usedBuildingBlocksTreeNode, _presenter);
      }

      [Observation]
      public void should_load_the_simulation_containing_those_building_blocks()
      {
         A.CallTo(() => _executionContext.Load(_simulation1)).MustHaveHappened();
         A.CallTo(() => _executionContext.Load(_simulation2)).MustHaveHappened();
      }

      [Observation]
      public void should_start_a_comparison_using_the_simulation_names_and_the_building_block_names()
      {
         _command.ObjectNames.ShouldOnlyContainInOrder(
            CoreConstants.ContainerName.BuildingBlockInSimulationNameFor(_buildingBlock1.Name, _simulation1.Name),
            CoreConstants.ContainerName.BuildingBlockInSimulationNameFor(_buildingBlock2.Name, _simulation2.Name)
            ); 
      }
   }

   public class When_creating_a_context_menu_for_a_set_of_used_building_blocks_that_do_not_support_comparison : concern_for_MultipleUsedBuildingBlockNodeContextMenuFactory
   {
      private Simulation _simulation1;
      private Simulation _simulation2;

      protected override void Context()
      {
         base.Context();
         var usedBuildingBlockNode1 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => usedBuildingBlockNode1.TagAsObject).Returns(_usedIndividual1);
         var usedBuildingBlockNode2 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => usedBuildingBlockNode2.TagAsObject).Returns(_usedIndividual2);
         _usedBuildingBlocksTreeNode.Add(usedBuildingBlockNode1);
         _usedBuildingBlocksTreeNode.Add(usedBuildingBlockNode2);

         _simulation1 = new IndividualSimulation().WithName("S1");
         _simulation2 = new IndividualSimulation().WithName("S2");

         _simulation1.AddUsedBuildingBlock(_usedIndividual1);
         _simulation2.AddUsedBuildingBlock(_usedIndividual2);
         A.CallTo(() => _buildingBlockInProjectManager.SimulationUsing(_usedIndividual1)).Returns(_simulation1);
         A.CallTo(() => _buildingBlockInProjectManager.SimulationUsing(_usedIndividual2)).Returns(_simulation2);

         A.CallTo(() => _buildingBlock1.BuildingBlockType).Returns(PKSimBuildingBlockType.Population);
      }

      protected override void Because()
      {
         sut.CreateFor(_usedBuildingBlocksTreeNode, _presenter);
      }

      [Observation]
      public void should_load_the_simulation_containing_those_building_blocks()
      {
         A.CallTo(() => _executionContext.Load(_simulation1)).MustHaveHappened();
         A.CallTo(() => _executionContext.Load(_simulation2)).MustHaveHappened();
      }

      [Observation]
      public void should_not_start_a_comparison_using_the_simulation_names_and_the_building_block_names()
      {
         A.CallTo(() => _container.Resolve<CompareObjectsUICommand>()).MustNotHaveHappened();
      }
   }

   public class When_checking_if_the_used_building_blocks_node_can_be_used_to_create_a_context_menu : concern_for_MultipleUsedBuildingBlockNodeContextMenuFactory
   {
      private ITreeNode<UsedBuildingBlock> _usedIndividualNode1;
      private ITreeNode<UsedBuildingBlock> _usedIndividualNode2;
      private ITreeNode<UsedBuildingBlock> _usedCompoundNode1;

      protected override void Context()
      {
         base.Context();
         _usedIndividualNode1 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => _usedIndividualNode1.TagAsObject).Returns(_usedIndividual1);
         _usedIndividualNode2 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => _usedIndividualNode2.TagAsObject).Returns(_usedIndividual2);
         _usedCompoundNode1 = A.Fake<ITreeNode<UsedBuildingBlock>>();
         A.CallTo(() => _usedCompoundNode1.TagAsObject).Returns(_usedCompound1);
      }

      [Observation]
      public void should_return_true_if_they_have_the_same_building_block_type()
      {
         _usedBuildingBlocksTreeNode.Add(_usedIndividualNode1);
         _usedBuildingBlocksTreeNode.Add(_usedIndividualNode2);
         sut.IsSatisfiedBy(_usedBuildingBlocksTreeNode, _presenter).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         _usedBuildingBlocksTreeNode.Add(_usedIndividualNode1);
         _usedBuildingBlocksTreeNode.Add(_usedCompoundNode1);
         sut.IsSatisfiedBy(_usedBuildingBlocksTreeNode, _presenter).ShouldBeFalse();
      }
   }
}