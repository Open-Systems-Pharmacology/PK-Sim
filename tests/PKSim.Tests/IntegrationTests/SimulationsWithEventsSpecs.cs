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

   public class When_creating_a_simulation_with_a_meal_event : concern_for_IndividualSimulation
   {
      private const string _mealEventName = "MyMeal";
      private const string _mealVolume = "Meal volume";
      private const string _mealVolumeReferenceAdult = "Meal volume (reference Adult)";
      private const string _volumeReferenceAdult = "Volume (reference Adult)";

      private PKSimEvent _mealEvent;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var eventMappingFactory = IoC.Resolve<IEventMappingFactory>();
         var eventFactory = IoC.Resolve<IEventFactory>();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         _mealEvent = eventFactory.Create(CoreConstantsForSpecs.Events.STANDARD_MEAL).WithName(_mealEventName);
         var eventMapping = eventMappingFactory.Create(_mealEvent);
         eventMapping.StartTime.ValueInDisplayUnit = 0;
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_mealEvent.Id, PKSimBuildingBlockType.Event) {BuildingBlock = _mealEvent});
         _simulation.EventProperties.AddEventMapping(eventMapping);

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      private IContainer mealEventContainer => _simulation.Model.Root.EntityAt<IContainer>(Constants.EVENTS, _mealEventName);

      private IContainer lumenStomach => _simulation.Model.Root.EntityAt<IContainer>(Constants.ORGANISM, CoreConstants.Organ.LUMEN, CoreConstants.Organ.STOMACH);

      [Observation]
      public void should_define_the_reference_meal_volume_in_the_event_building_block_instead_of_the_meal_volume()
      {
         _mealEvent.Parameter(_mealVolumeReferenceAdult).ShouldNotBeNull();
         _mealEvent.Parameter(_mealVolume).ShouldBeNull();
      }

      [Observation]
      public void should_create_the_meal_volume_parameter_in_the_simulation()
      {
         var mealVolume = mealEventContainer.EntityAt<IParameter>(_mealVolume);
         mealVolume.ShouldNotBeNull();
         mealVolume.Visible.ShouldBeTrue();
         mealVolume.Editable.ShouldBeTrue();
         mealVolume.CanBeVariedInPopulation.ShouldBeFalse();
      }

      [Observation]
      public void should_create_a_hidden_reference_adult_volume_parameter_in_the_lumen_stomach()
      {
         var referenceVolume = lumenStomach.EntityAt<IParameter>(_volumeReferenceAdult);
         referenceVolume.ShouldNotBeNull();
         referenceVolume.Visible.ShouldBeFalse();

         //standard individual is a 30 years old european male: reference volume and stomach volume should be equal
         referenceVolume.Value.ShouldBeEqualTo(lumenStomach.Parameter(Constants.Parameters.VOLUME).Value, 1e-5);
      }

      [Observation]
      public void should_scale_the_meal_volume_with_the_stomach_volume()
      {
         var mealVolume = mealEventContainer.EntityAt<IParameter>(_mealVolume);
         var referenceMealVolume = mealEventContainer.EntityAt<IParameter>(_mealVolumeReferenceAdult);
         var stomachVolume = lumenStomach.Parameter(Constants.Parameters.VOLUME);
         var referenceStomachVolume = lumenStomach.EntityAt<IParameter>(_volumeReferenceAdult);

         mealVolume.Value.ShouldBeEqualTo(referenceMealVolume.Value * stomachVolume.Value / referenceStomachVolume.Value, 1e-5);
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

      [Observation]
      public void should_create_event_group_for_protocol_event()
      {
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         var simEvent = _simulation.Model.Root.EntityAt<IContainer>(Constants.EVENTS, "MyEvent");
         simEvent.ShouldNotBeNull();
      }
   }
}