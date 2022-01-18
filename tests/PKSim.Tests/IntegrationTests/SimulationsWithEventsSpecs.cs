using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
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

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>() ;
         var pksimEvent = eventFactory.Create(CoreConstantsForSpecs.Events.URINARY_BLADDER_EMPTYING).WithName("Event");
         var eventMapping = eventMappingFactory.Create(pksimEvent);
         eventMapping.StartTime.ValueInDisplayUnit = 2;
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(pksimEvent.Id, PKSimBuildingBlockType.Event){BuildingBlock = pksimEvent});
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
}