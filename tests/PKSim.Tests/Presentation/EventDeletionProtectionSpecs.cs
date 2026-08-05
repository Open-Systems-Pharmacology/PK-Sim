using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EventDeletionProtection : concern_for_BuildingBlockTask
   {
      protected PKSimEvent _pkSimEvent;
      protected Simulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         //use the real building block in project manager so that the deletion check runs against the actual event mappings
         var buildingBlockInProjectManager = new BuildingBlockInProjectManager(_buildingBlockRepository, A.Fake<IEventPublisher>());
         sut = new BuildingBlockTask(
            _executionContext,
            _applicationController,
            _dialogCreator,
            buildingBlockInProjectManager,
            _entityTask,
            _templateTaskQuery,
            _singleStartPresenterTask,
            _buildingBlockRepository,
            _presenterSettingsTask,
            _simulationReferenceUpdater);

         _pkSimEvent = new PKSimEvent().WithId("eventId").WithName("Meal");
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()}.WithName("S1");

         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {_simulation});
         A.CallTo(() => _buildingBlockRepository.All(A<Func<Simulation, bool>>._))
            .ReturnsLazily(x => new[] {_simulation}.Where(x.GetArgument<Func<Simulation, bool>>(0)).ToList());
      }
   }

   public class When_deleting_a_pksim_event_mapped_in_a_simulation_via_an_event_placeholder_mapping : concern_for_EventDeletionProtection
   {
      protected override async Task Context()
      {
         await base.Context();
         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping {EventKey = "EVENT_1", TemplateEventId = _pkSimEvent.Id});
         _simulation.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_not_allow_the_deletion()
      {
         The.Action(() => sut.Delete(_pkSimEvent)).ShouldThrowAn<CannotDeleteBuildingBlockException>();
      }
   }

   public class When_deleting_a_pksim_event_not_mapped_in_any_simulation : concern_for_EventDeletionProtection
   {
      private bool _result;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).Returns(ViewResult.Yes);
      }

      protected override Task Because()
      {
         _result = sut.Delete(_pkSimEvent);
         return _completed;
      }

      [Observation]
      public void should_delete_the_event()
      {
         _result.ShouldBeTrue();
      }
   }
}
