using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Services;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;
using TreeNodeFactory = PKSim.Presentation.Nodes.TreeNodeFactory;


namespace PKSim.Presentation
{
   public abstract class concern_for_TreeNodeFactory : ContextSpecification<ITreeNodeFactory>
   {
      protected IBuildingBlockRetriever _buildingBlockRetriever;
      private IToolTipPartCreator _toolTipPartCreator;
      protected IObservedDataRepository _observedDataRepository;

      protected override void Context()
      {
         _buildingBlockRetriever = A.Fake<IBuildingBlockRetriever>();
         _observedDataRepository = A.Fake<IObservedDataRepository>();
         _toolTipPartCreator = A.Fake<IToolTipPartCreator>();
         sut = new TreeNodeFactory(_buildingBlockRetriever, _observedDataRepository, _toolTipPartCreator);
      }
   }

   public class When_asked_to_create_a_node_for_a_specific_node_type : concern_for_TreeNodeFactory
   {
      private ITreeNode _result;
      private RootNodeType _rootNodeType;

      [Observation]
      public void should_return_a_valid_tree_node()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_set_the_id_and_names_of_the_created_node()
      {
         _result.Id.ShouldBeEqualTo(_rootNodeType.Id);
         _result.Text.ShouldBeEqualTo(_rootNodeType.Name);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_rootNodeType);
      }

      protected override void Context()
      {
         base.Context();
         _rootNodeType = PKSimRootNodeTypes.IndividualFolder;
      }
   }

   public class When_asked_to_retrieve_a_node_for_a_certain_type : concern_for_TreeNodeFactory
   {
      private ITreeNode<Simulation> _result;
      private Simulation _simulation;

      [Observation]
      public void should_set_the_entity_in_the_returned_node()
      {
         _result.Tag.ShouldBeEqualTo(_simulation);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
      }
   }

   public class When_asked_to_retrieve_a_text_node : concern_for_TreeNodeFactory
   {
      private ITreeNode _result;

      protected override void Because()
      {
         _result = sut.CreateFor("toto");
      }

      [Observation]
      public void should_return_a_node_for_which_the_text_has_been_set_to_the_given_display_name()
      {
         _result.Text.ShouldBeEqualTo("toto");
      }
   }

   public class When_creating_a_building_block_node_for_a_simulation_and_a_building_block_id : concern_for_TreeNodeFactory
   {
      private ITreeNode _result;
      private Simulation _simulation;
      private UsedBuildingBlock _bb;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _bb = new UsedBuildingBlock("toto", PKSimBuildingBlockType.Compound);
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_bb.TemplateId)).Returns(A.Fake<IPKSimBuildingBlock>());
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation, _bb);
      }

      [Observation]
      public void should_retrieve_the_building_block_from_the_building_block_id()
      {
         A.CallTo(() => _buildingBlockRetriever.BuildingBlockWithId(_bb.TemplateId)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_building_block_node()
      {
         _result.ShouldBeAnInstanceOf<UsedBuildingBlockInSimulationNode>();
      }
   }

   public class When_creating_a_node_for_some_used_observed_data : concern_for_TreeNodeFactory
   {
      private UsedObservedData _usedObservedData;
      private ITreeNode _result;
      private DataRepository _dataRepository;

      protected override void Because()
      {
         _usedObservedData = new UsedObservedData {Id = "toto"};
         _dataRepository = new DataRepository(_usedObservedData.Id);
         A.CallTo(() => _observedDataRepository.FindFor(_usedObservedData)).Returns(_dataRepository);
         _result = sut.CreateFor(_usedObservedData);
      }

      [Observation]
      public void should_return_a_node_whose_tag_was_set_to_the_observed_data_in_the_project()
      {
         _result.TagAsObject.ShouldBeEqualTo(_usedObservedData);
      }

      [Observation]
      public void should_return_a_used_observed_data_node()
      {
         _result.ShouldBeAnInstanceOf<UsedObservedDataNode>();
      }

      [Observation]
      public void the_id_of_the_returned_node_should_not_be_the_id_of_the_observed_data_repository()
      {
         _result.Id.ShouldNotBeEqualTo(_dataRepository.Id);
      }
   }
}