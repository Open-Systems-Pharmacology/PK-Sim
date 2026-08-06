using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_EventFromMappingRetriever : ContextSpecification<IEventFromMappingRetriever>
   {
      protected Simulation _simulation;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _simulation = new IndividualSimulation();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         sut = new EventFromMappingRetriever(_buildingBlockInProjectManager, _buildingBlockRepository);
      }
   }

   public class When_retrieving_a_template_event_used_for_a_mapping : concern_for_EventFromMappingRetriever
   {
      private PKSimEvent _event1;
      private PKSimEvent _event2;
      private PKSimEvent _templateEvent;

      protected override void Context()
      {
         base.Context();
         _templateEvent = new PKSimEvent().WithId("template");
         _event1 = new PKSimEvent().WithName("e1").WithId("1");
         _event2 = new PKSimEvent().WithName("e2").WithId("2");
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_event1.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _event1 });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_event2.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _event2 });
         A.CallTo(() => _buildingBlockRepository.ById<PKSimEvent>("template")).Returns(_templateEvent);
         A.CallTo(() => _buildingBlockRepository.ById<PKSimEvent>("does not exist")).Returns(null);
      }

      [Observation]
      public void should_return_null_if_the_given_mapping_is_null()
      {
         sut.TemplateEventUsedBy(_simulation, null).ShouldBeNull();
      }

      [Observation]
      public void should_return_null_if_the_mapping_has_no_template_id()
      {
         sut.TemplateEventUsedBy(_simulation, new EventPlaceholderMapping()).ShouldBeNull();
      }

      [Observation]
      public void should_return_the_template_event_if_the_simulation_was_not_initialized_with_that_event()
      {
         sut.TemplateEventUsedBy(_simulation, new EventPlaceholderMapping { TemplateEventId = "template" }).ShouldBeEqualTo(_templateEvent);
      }

      [Observation]
      public void should_return_null_if_the_template_id_is_not_found()
      {
         sut.TemplateEventUsedBy(_simulation, new EventPlaceholderMapping { TemplateEventId = "does not exist" }).ShouldBeNull();
      }
   }

   public class When_retrieving_a_template_event_that_has_been_used_in_the_simulation : concern_for_EventFromMappingRetriever
   {
      private PKSimEvent _localEvent;
      private PKSimEvent _result;

      protected override void Context()
      {
         base.Context();
         _localEvent = new PKSimEvent().WithId("local");
         var usedBuildingBlock = new UsedBuildingBlock("e1", PKSimBuildingBlockType.Event) { BuildingBlock = _localEvent };
         _simulation.AddUsedBuildingBlock(usedBuildingBlock);
         A.CallTo(() => _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<PKSimEvent>(usedBuildingBlock)).Returns(_localEvent);
      }

      protected override void Because()
      {
         _result = sut.TemplateEventUsedBy(_simulation, new EventPlaceholderMapping { TemplateEventId = "e1" });
      }

      [Observation]
      public void should_return_the_actual_building_block()
      {
         _result.ShouldBeEqualTo(_localEvent);
      }
   }
}
