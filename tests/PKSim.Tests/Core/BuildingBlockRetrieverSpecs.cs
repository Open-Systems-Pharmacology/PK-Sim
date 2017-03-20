using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockRetriever : ContextSpecification<IBuildingBlockRetriever>
   {
      protected IEntity _objectWithoutParent;
      protected Individual _individual;
      private IContainer _container;
      protected IEntity _objectWithParent;
      protected IWithIdRepository _withIdRepository;

      protected override void Context()
      {
         _withIdRepository = A.Fake<IWithIdRepository>();
         sut = new BuildingBlockRetriever(_withIdRepository);
         _objectWithoutParent = A.Fake<IEntity>();
         _objectWithParent = A.Fake<IEntity>();
         _objectWithoutParent.ParentContainer = null;
         _container = new Compartment();
         _container.Add(_objectWithParent);
         _individual = new Individual();
         _individual.Add(_container);
      }
   }

   public class When_retrieving_the_building_block_containing_a_given_entity : concern_for_BuildingBlockRetriever
   {
      [Observation]
      public void should_return_the_first_parent_in_the_hierarchy_which_is_defined_as_a_building_block()
      {
         sut.BuildingBlockContaining(_objectWithParent).ShouldBeEqualTo(_individual);
      }

      [Observation]
      public void shoul_return_null_if_entity_is_not_attached_to_any_building_block()
      {
         sut.BuildingBlockContaining(_objectWithoutParent).ShouldBeNull();
      }
   }

   public class When_retrieving_the_building_id_containing_a_given_entity : concern_for_BuildingBlockRetriever
   {
      [Observation]
      public void should_return_the_id_of_the_first_parent_in_the_hierarchy_which_is_defined_as_a_building_block()
      {
         sut.BuildingBlockIdContaining(_objectWithParent).ShouldBeEqualTo(_individual.Id);
      }

      [Observation]
      public void should_return_an_empty_string_if_the_entity_is_not_attached_to_any_parent()
      {
         sut.BuildingBlockIdContaining(_objectWithoutParent).IsNullOrEmpty().ShouldBeTrue();
      }
   }

   public class When_retrieving_the_building_block_by_id : concern_for_BuildingBlockRetriever
   {
      private string _id;
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _id = "tralala";
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_id)).Returns(true);
         A.CallTo(() => _withIdRepository.Get<IPKSimBuildingBlock>(_id)).Returns(_buildingBlock);
      }

      [Observation]
      public void should_return_the_building_block_registered_with_this_id()
      {
         sut.BuildingBlockWithId(_id).ShouldBeEqualTo(_buildingBlock);
      }
   }

   public class When_retrieving_the_building_block_by_id_and_type : concern_for_BuildingBlockRetriever
   {
      private string _id;
      private Individual _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _id = "tralala";
         _buildingBlock = A.Fake<Individual>();
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_id)).Returns(true);
         A.CallTo(() => _withIdRepository.Get<IPKSimBuildingBlock>(_id)).Returns(_buildingBlock);
      }

      [Observation]
      public void should_return_the_building_block_registered_with_this_id_and_the_type_if_registered()
      {
         sut.BuildingBlockWithId<Individual>(_id).ShouldBeEqualTo(_buildingBlock);
      }

      [Observation]
      public void should_return_null_if_the_id_was_registered_for_another_type()
      {
         sut.BuildingBlockWithId<Compound>(_id).ShouldBeNull();
      }

      [Observation]
      public void should_return_null_if_the_id_was_not_registered()
      {
         sut.BuildingBlockWithId<Individual>("toto").ShouldBeNull();
      }
   }

   public class When_retrieving_a_building_block_for_an_entity_defined_in_a_model_container_within_a_simulation : concern_for_BuildingBlockRetriever
   {
      private Simulation _simulation;
      private IEntity _entity;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();
         _entity = new PKSimParameter();
         _simulation.Model = new OSPSuite.Core.Domain.Model();
         _simulation.Model.Root = new Container();
         _simulation.Model.Root.Add(_entity);
      }

      [Observation]
      public void should_return_null()
      {
         sut.BuildingBlockContaining(_entity).ShouldBeNull();
      }
   }
}