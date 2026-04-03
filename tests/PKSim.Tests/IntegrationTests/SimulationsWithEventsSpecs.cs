using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using System.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationsWithEvents : concern_for_IndividualSimulation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         var eventMappingFactory = IoC.Resolve<IEventMappingFactory>();
         var eventFactory = IoC.Resolve<IEventFactory>();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         var pksimEvent = eventFactory.Create(CoreConstantsForSpecs.Events.URINARY_BLADDER_EMPTYING).WithName("Event");
         var eventMapping = eventMappingFactory.Create(pksimEvent);
         eventMapping.StartTime.ValueInDisplayUnit = 2;
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(pksimEvent.Id, PKSimBuildingBlockType.Event) {BuildingBlock = pksimEvent});
         _simulation.EventProperties.AddEventMapping(eventMapping);
      }
   }

   public class When_creating_a_simulation_with_the_urine_emptying_event : concern_for_SimulationsWithEvents
   {
      [Observation]
      public void should_be_able_to_create_a_simulation()
      {
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         var simEvent = _simulation.Model.Root.EntityAt<IContainer>(Constants.EVENTS, "Event");
         simEvent.ShouldNotBeNull();
      }
   }

   public class When_creating_a_simulation_with_a_simple_protocol_that_has_an_event_placeholder : concern_for_IndividualSimulation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         var eventMappingFactory = IoC.Resolve<IEventMappingFactory>();
         var eventFactory = IoC.Resolve<IEventFactory>();

         // Set event key on the simple protocol to simulate "Administer with event" checkbox
         var simpleProtocol = _protocol.DowncastTo<SimpleProtocol>();
         simpleProtocol.EventKey = CoreConstants.DEFAULT_EVENT_KEY;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();

         // Map the event placeholder to an actual event building block
         var pksimEvent = eventFactory.Create(CoreConstantsForSpecs.Events.URINARY_BLADDER_EMPTYING).WithName("MyEvent");
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(pksimEvent.Id, PKSimBuildingBlockType.Event) { BuildingBlock = pksimEvent });

         var protocolProperties = _simulation.CompoundPropertiesList.First().ProtocolProperties;
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = CoreConstants.DEFAULT_EVENT_KEY,
            TemplateEventId = pksimEvent.Id,
            Event = pksimEvent
         });
      }

      [Observation]
      public void should_be_able_to_create_the_simulation_without_crashing()
      {
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         _simulation.Model.ShouldNotBeNull();
      }
   }
}