using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockInSimulationManager : ContextSpecification<IBuildingBlockInSimulationManager>
   {
      protected IEventPublisher _eventPublisher;
      protected Simulation _simulation1;
      protected Simulation _simulation2;
      protected Simulation _simulation3;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IPKSimBuildingBlock _templateBuildingBlock;
      protected string _id;
      protected UsedBuildingBlock _usedBuildingBlock;

      protected override void Context()
      {
         _eventPublisher = A.Fake<IEventPublisher>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _simulation1 = new IndividualSimulation();
         _simulation2 = new IndividualSimulation();
         _simulation3 = new IndividualSimulation();
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {_simulation1, _simulation2, _simulation3});
         _id = "tralala";
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>().WithId(_id);
         _usedBuildingBlock = new UsedBuildingBlock(_templateBuildingBlock.Id, _templateBuildingBlock.BuildingBlockType);
         A.CallTo(() => _buildingBlockRepository.ById(_templateBuildingBlock.Id)).Returns(_templateBuildingBlock);
         sut = new BuildingBlockInSimulationManager(_buildingBlockRepository, _eventPublisher);
      }
   }

   public class When_updating_the_simulation_status_for_simulations_using_a_given_building_block : concern_for_BuildingBlockInSimulationManager
   {
      private List<SimulationStatusChangedEvent> _events;

      protected override void Context()
      {
         base.Context();
         _events = new List<SimulationStatusChangedEvent>();
         _simulation1.AddUsedBuildingBlock(_usedBuildingBlock);
         _simulation3.AddUsedBuildingBlock(_usedBuildingBlock);

         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationStatusChangedEvent>._)).Invokes(
            x => _events.Add(x.GetArgument<SimulationStatusChangedEvent>(0)));
      }

      protected override void Because()
      {
         sut.UpdateStatusForSimulationUsing(_templateBuildingBlock);
      }

      [Observation]
      public void should_notify_the_simulation_status_changed_event_for_each_simulation_using_that_building_block()
      {
         _events.Count.ShouldBeEqualTo(2);
         var simulationEvent1 = _events[0];
         simulationEvent1.Simulation.ShouldBeEqualTo(_simulation1);
         var simulationEvent2 = _events[1];
         simulationEvent2.Simulation.ShouldBeEqualTo(_simulation3);
      }
   }

   public class When_updating_the_simulation_status_for_a_simulation : concern_for_BuildingBlockInSimulationManager
   {
      private Simulation _simulation;
      private IList<SimulationStatusChangedEvent> _events;

      protected override void Context()
      {
         base.Context();
         _events = new List<SimulationStatusChangedEvent>();
         _simulation = new IndividualSimulation();
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationStatusChangedEvent>.Ignored)).Invokes(
            x => _events.Add(x.GetArgument<SimulationStatusChangedEvent>(0)));
      }

      protected override void Because()
      {
         sut.UpdateStatusForSimulation(_simulation);
      }

      [Observation]
      public void should_notify_the_simulation_status_changed_event_for_each_simulation_using_that_building_block()
      {
         var simulationEvent1 = _events[0];
         simulationEvent1.Simulation.ShouldBeEqualTo(_simulation);
      }
   }

   public class When_the_simulation_using_building_block_retriever_is_asked_to_retrieve_all_simulations_using_a_building_block : concern_for_BuildingBlockInSimulationManager
   {
      private IPKSimBuildingBlock _bb1;
      private IPKSimBuildingBlock _bb2;
      private UsedBuildingBlock _usedBuildingBlock1;
      private UsedBuildingBlock _usedBuildingBlock2;

      protected override void Context()
      {
         base.Context();
         _bb1 = A.Fake<IPKSimBuildingBlock>();
         _bb2 = A.Fake<IPKSimBuildingBlock>();
         _bb1.Id = "Tralala";
         _bb2.Id = "trilil";
         _usedBuildingBlock1 = new UsedBuildingBlock(_bb1.Id, _bb1.BuildingBlockType);
         _usedBuildingBlock2 = new UsedBuildingBlock(_bb2.Id, _bb2.BuildingBlockType);

         _simulation1.AddUsedBuildingBlock(_usedBuildingBlock1);
         _simulation2.AddUsedBuildingBlock(_usedBuildingBlock2);
         _simulation3.AddUsedBuildingBlock(_usedBuildingBlock1);
         _simulation3.AddUsedBuildingBlock(_usedBuildingBlock2);
      }

      [Observation]
      public void should_return_all_simulation_defined_in_project_that_reference_the_given_building_block()
      {
         sut.SimulationsUsing(_bb2).ShouldOnlyContain(_simulation2, _simulation3);
         sut.SimulationsUsing(_bb1).ShouldOnlyContain(_simulation1, _simulation3);
      }
   }

   public class When_retrieving_the_building_block_status_for_a_building_block_id_whose_version_has_changed : concern_for_BuildingBlockInSimulationManager
   {
      protected override void Context()
      {
         base.Context();
         _usedBuildingBlock.Altered = false;
         _templateBuildingBlock.Version = 5;
      }

      [Observation]
      public void should_return_the_red_status()
      {
         sut.StatusFor(_usedBuildingBlock).ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public class When_retrieving_the_building_block_status_for_a_building_block_id_whose_template_does_not_exist : concern_for_BuildingBlockInSimulationManager
   {
      protected override void Context()
      {
         base.Context();
         _usedBuildingBlock = new UsedBuildingBlock("does_not_exist", _templateBuildingBlock.BuildingBlockType);
         A.CallTo(() => _buildingBlockRepository.ById(_usedBuildingBlock.TemplateId)).Returns(null);
      }

      [Observation]
      public void should_return_the_red_status()
      {
         sut.StatusFor(_usedBuildingBlock).ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public class When_retrieving_the_building_block_status_for_a_building_block_id_whose_version_is_the_same_as_the_building_block_and_that_was_not_alterd_in_the_simulation :
      concern_for_BuildingBlockInSimulationManager
   {
      private BuildingBlockStatus _result;

      protected override void Context()
      {
         base.Context();
         _usedBuildingBlock.Altered = false;
         _usedBuildingBlock.Version = _templateBuildingBlock.Version;
      }

      protected override void Because()
      {
         _result = sut.StatusFor(_usedBuildingBlock);
      }

      [Observation]
      public void should_return_the_green_status_if_the_version_of_the_building_block_are_the_same_and_the_building_block_was_not_altered_in_the_simulation()
      {
         _result.ShouldBeEqualTo(BuildingBlockStatus.Green);
      }
   }

   public class When_retrieving_the_building_block_status_for_a_building_block_id_that_was_not_alterd_in_the_simulation : concern_for_BuildingBlockInSimulationManager
   {
      private BuildingBlockStatus _resultWithSameVersion;
      private BuildingBlockStatus _resultWithDifferentVersion;

      protected override void Context()
      {
         base.Context();
         _usedBuildingBlock.Altered = true;
         _usedBuildingBlock.Version = _templateBuildingBlock.Version;
      }

      protected override void Because()
      {
         _resultWithSameVersion = sut.StatusFor(_usedBuildingBlock);
         _usedBuildingBlock = new UsedBuildingBlock(_templateBuildingBlock.Id, _templateBuildingBlock.BuildingBlockType) {Altered = true};
         _resultWithDifferentVersion = sut.StatusFor(_usedBuildingBlock);
      }

      [Observation]
      public void should_return_the_red_status_in_any_case()
      {
         _resultWithSameVersion.ShouldBeEqualTo(BuildingBlockStatus.Red);
         _resultWithDifferentVersion.ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public abstract class concern_for_building_block_status_retriever_for_simulation : concern_for_BuildingBlockInSimulationManager
   {
      protected BuildingBlockStatus _result;
      protected Simulation _simulation;
      protected UsedBuildingBlock _usedBuildingBlockId1;
      protected UsedBuildingBlock _usedBuildingBlockId2;
      protected UsedBuildingBlock _usedBuildingBlockId3;
      protected IPKSimBuildingBlock _bb1;
      protected IPKSimBuildingBlock _bb2;
      protected IPKSimBuildingBlock _bb3;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _usedBuildingBlockId1 = new UsedBuildingBlock("bb1", PKSimBuildingBlockType.Compound);
         _usedBuildingBlockId2 = new UsedBuildingBlock("bb2", PKSimBuildingBlockType.Compound);
         _usedBuildingBlockId3 = new UsedBuildingBlock("bb3", PKSimBuildingBlockType.Compound);
         _bb1 = A.Fake<IPKSimBuildingBlock>();
         _bb1.Version = 1;
         _bb1.Id = _usedBuildingBlockId1.TemplateId;
         _bb2 = A.Fake<IPKSimBuildingBlock>();
         _bb2.Version = 1;
         _bb2.Id = _usedBuildingBlockId2.TemplateId;
         _bb3 = A.Fake<IPKSimBuildingBlock>();
         _bb3.Version = 1;
         _bb3.Id = _usedBuildingBlockId3.TemplateId;

         A.CallTo(() => _buildingBlockRepository.ById(_usedBuildingBlockId1.TemplateId)).Returns(_bb1);
         A.CallTo(() => _buildingBlockRepository.ById(_usedBuildingBlockId2.TemplateId)).Returns(_bb2);
         A.CallTo(() => _buildingBlockRepository.ById(_usedBuildingBlockId3.TemplateId)).Returns(_bb3);
         A.CallTo(() => _simulation.UsedBuildingBlocks).Returns(new[] {_usedBuildingBlockId1, _usedBuildingBlockId2, _usedBuildingBlockId3});
      }

      protected override void Because()
      {
         _result = sut.StatusFor(_simulation);
      }
   }

   public class When_retrieving_the_status_of_an_imported_simulation : concern_for_BuildingBlockInSimulationManager
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.IsImported).Returns(true);
      }

      [Observation]
      public void should_always_return_the_green_status()
      {
         sut.StatusFor(_simulation).ShouldBeEqualTo(BuildingBlockStatus.Green);
      }
   }

   public class When_retrieving_the_status_of_a_simulation_for_which_all_used_building_blocks_are_uptodate : concern_for_building_block_status_retriever_for_simulation
   {
      protected override void Context()
      {
         base.Context();
         _usedBuildingBlockId1.Version = _bb1.Version;
         _usedBuildingBlockId2.Version = _bb2.Version;
         _usedBuildingBlockId3.Version = _bb3.Version;
         _usedBuildingBlockId1.Altered = false;
         _usedBuildingBlockId2.Altered = false;
         _usedBuildingBlockId3.Altered = false;
      }

      [Observation]
      public void should_return_a_green_status()
      {
         _result.ShouldBeEqualTo(BuildingBlockStatus.Green);
      }
   }

   public class When_retrieving_the_status_of_a_simulation_for_which_all_used_building_blocks_are_uptodate_but_one_was_altered : concern_for_building_block_status_retriever_for_simulation
   {
      protected override void Context()
      {
         base.Context();
         _usedBuildingBlockId1.Version = _bb1.Version;
         _usedBuildingBlockId2.Version = _bb2.Version;
         _usedBuildingBlockId3.Version = _bb3.Version;
         _usedBuildingBlockId1.Altered = false;
         _usedBuildingBlockId2.Altered = true;
         _usedBuildingBlockId3.Altered = false;
      }

      [Observation]
      public void should_return_a_red_status()
      {
         _result.ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public class When_retrieving_the_status_of_a_simulation_for_which_one_building_block_is_not_uptodate : concern_for_building_block_status_retriever_for_simulation
   {
      protected override void Context()
      {
         base.Context();
         _usedBuildingBlockId1.Version = _bb1.Version;
         _usedBuildingBlockId2.Version = 3;
         _usedBuildingBlockId3.Version = _bb3.Version;
         _usedBuildingBlockId1.Altered = false;
         _usedBuildingBlockId2.Altered = true;
         _usedBuildingBlockId3.Altered = false;
      }

      [Observation]
      public void should_return_a_red_status()
      {
         _result.ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public class When_retrieving_the_template_building_block_used_to_create_a_simulation_for_a_given_building_block : concern_for_BuildingBlockInSimulationManager
   {
      private IPKSimBuildingBlock _buildingBlockInSimulation;

      protected override void Context()
      {
         base.Context();
         _buildingBlockInSimulation = A.Fake<IPKSimBuildingBlock>();
         _usedBuildingBlock.BuildingBlock = _buildingBlockInSimulation;
         _simulation1.AddUsedBuildingBlock(_usedBuildingBlock);
      }

      [Observation]
      public void should_return_the_template_building_block_if_the_building_block_versions_are_the_same()
      {
         _usedBuildingBlock.Version = _templateBuildingBlock.Version;
         sut.TemplateBuildingBlockUsedBy(_simulation1, _buildingBlockInSimulation).ShouldBeEqualTo(_templateBuildingBlock);
      }

      [Observation]
      public void should_return_the_used_building_block_if_the_building_block_versions_are_not_the_same()
      {
         _usedBuildingBlock.Version = _templateBuildingBlock.Version + 1;
         sut.TemplateBuildingBlockUsedBy(_simulation1, _buildingBlockInSimulation).ShouldBeEqualTo(_buildingBlockInSimulation);
      }
   }

   public class When_retrieving_the_simulation_using_a_given_used_building_block : concern_for_BuildingBlockInSimulationManager
   {
      protected override void Context()
      {
         base.Context();
         var buildingBlockInSimulation = A.Fake<IPKSimBuildingBlock>().WithId("xx");
         _usedBuildingBlock.BuildingBlock = buildingBlockInSimulation;
         _simulation1.AddUsedBuildingBlock(_usedBuildingBlock);
      }

      [Observation]
      public void should_return_the_simulation_containing_the_used_building_block_if_defined()
      {
         sut.SimulationUsing(_usedBuildingBlock).ShouldBeEqualTo(_simulation1);
      }

      [Observation]
      public void should_return_null_if_no_simulation_was_found_using_the_given_used_building_block()
      {
         sut.SimulationUsing(new UsedBuildingBlock("Temp", PKSimBuildingBlockType.Individual){Id = "xxx"}).ShouldBeNull();
      }
   }
}