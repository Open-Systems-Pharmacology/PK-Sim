using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundProtocolEventPresenter : ContextSpecification<ISimulationCompoundProtocolEventPresenter>
   {
      protected ISimulationCompoundProtocolEventView _view;
      protected IEventTask _eventTask;
      protected IEventFromMappingRetriever _eventFromMappingRetriever;
      protected IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      protected Protocol _protocol;
      protected Simulation _simulation;
      protected ProtocolProperties _protocolProperties;
      protected PKSimEvent _event1;
      protected PKSimEvent _event2;
      protected Compound _compound;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProtocolEventView>();
         _eventTask = A.Fake<IEventTask>();
         _eventFromMappingRetriever = A.Fake<IEventFromMappingRetriever>();
         _buildingBlockSelectionDisplayer = A.Fake<IBuildingBlockSelectionDisplayer>();
         _event1 = new PKSimEvent().WithName("Event1").WithId("event1Id");
         _event2 = new PKSimEvent().WithName("Event2").WithId("event2Id");
         _protocol = A.Fake<Protocol>();
         _compound = A.Fake<Compound>();
         _simulation = A.Fake<Simulation>();
         _protocolProperties = new ProtocolProperties();
         var compoundProperties = new CompoundProperties();
         A.CallTo(() => _simulation.CompoundPropertiesFor(_compound)).Returns(compoundProperties);
         compoundProperties.ProtocolProperties = _protocolProperties;
         _protocolProperties.Protocol = _protocol;
         A.CallTo(() => _eventTask.All()).Returns(new[] { _event1, _event2 });
         sut = new SimulationCompoundProtocolEventPresenter(_view, _eventTask, _eventFromMappingRetriever, _buildingBlockSelectionDisplayer);
      }
   }

   public class When_the_event_presenter_is_editing_a_protocol_with_event_placeholders : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private string _eventKey1;
      private string _eventKey2;
      private IList<EventPlaceholderMappingDTO> _eventMappingDtoList;

      protected override void Context()
      {
         base.Context();
         _eventKey1 = "EVENT_1";
         _eventKey2 = "EVENT_2";
         A.CallTo(() => _protocol.UsedEventKeys).Returns(new[] { _eventKey1, _eventKey2 });
         A.CallTo(() => _view.BindTo(A<IEnumerable<EventPlaceholderMappingDTO>>._))
            .Invokes(x => _eventMappingDtoList = x.GetArgument<IEnumerable<EventPlaceholderMappingDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_display_event_mapping_for_each_event_key()
      {
         _eventMappingDtoList.Count.ShouldBeEqualTo(2);
         _eventMappingDtoList[0].EventKey.ShouldBeEqualTo(_eventKey1);
         _eventMappingDtoList[0].Event.ShouldNotBeNull();
         _eventMappingDtoList[1].EventKey.ShouldBeEqualTo(_eventKey2);
         _eventMappingDtoList[1].Event.ShouldNotBeNull();
      }

      [Observation]
      public void should_make_event_mapping_visible()
      {
         A.CallToSet(() => _view.EventVisible).To(true).MustHaveHappened();
      }

      [Observation]
      public void should_bind_to_the_view()
      {
         A.CallTo(() => _view.BindTo(A<IEnumerable<EventPlaceholderMappingDTO>>._)).MustHaveHappened();
      }
   }

   public class When_the_event_presenter_is_editing_a_protocol_without_event_placeholders : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private IList<EventPlaceholderMappingDTO> _eventMappingDtoList;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _protocol.UsedEventKeys).Returns(Enumerable.Empty<string>());
         A.CallTo(() => _view.BindTo(A<IEnumerable<EventPlaceholderMappingDTO>>._))
            .Invokes(x => _eventMappingDtoList = x.GetArgument<IEnumerable<EventPlaceholderMappingDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_not_display_any_event_mapping()
      {
         _eventMappingDtoList.Any().ShouldBeFalse();
      }

      [Observation]
      public void should_hide_event_mapping_view()
      {
         A.CallToSet(() => _view.EventVisible).To(false).MustHaveHappened();
      }
   }

   public class When_the_event_presenter_is_editing_a_protocol_that_is_not_defined : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private IList<EventPlaceholderMappingDTO> _eventMappingDtoList;

      protected override void Context()
      {
         base.Context();
         _protocolProperties.Protocol = null;
         A.CallTo(() => _view.BindTo(A<IEnumerable<EventPlaceholderMappingDTO>>._))
            .Invokes(x => _eventMappingDtoList = x.GetArgument<IEnumerable<EventPlaceholderMappingDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_not_display_any_event_mapping()
      {
         _eventMappingDtoList.Any().ShouldBeFalse();
      }
   }

   public class When_retrieving_all_events_for_a_placeholder_mapping : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private IEnumerable<EventSelectionDTO> _result;

      protected override void Because()
      {
         _result = sut.AllEventsFor(new EventPlaceholderMappingDTO());
      }

      [Observation]
      public void should_return_all_available_events()
      {
         _result.Select(x => x.BuildingBlock).ShouldOnlyContain(_event1, _event2);
      }
   }

   public class When_creating_a_new_event_for_a_placeholder_mapping : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private EventPlaceholderMappingDTO _dto;
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         _dto = new EventPlaceholderMappingDTO { EventKey = "EVENT_1" };
         A.CallTo(() => _eventTask.AddToProject()).Returns(_event1);
         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.CreateEventFor(_dto);
      }

      [Observation]
      public void should_set_the_created_event_in_the_mapping()
      {
         _dto.Event.ShouldBeEqualTo(_event1);
      }

      [Observation]
      public void should_refresh_the_view()
      {
         A.CallTo(() => _view.RefreshData()).MustHaveHappened();
      }

      [Observation]
      public void should_notify_status_changed()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

   public class When_saving_event_placeholder_configuration : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private string _eventKey1;

      protected override void Context()
      {
         base.Context();
         _eventKey1 = "EVENT_1";
         A.CallTo(() => _protocol.UsedEventKeys).Returns(new[] { _eventKey1 });
         sut.EditSimulation(_simulation, _compound);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_save_event_placeholder_mappings_to_protocol_properties()
      {
         _protocolProperties.EventPlaceholderMappings.Count.ShouldBeEqualTo(1);
         _protocolProperties.EventPlaceholderMappings[0].EventKey.ShouldBeEqualTo(_eventKey1);
         _protocolProperties.EventPlaceholderMappings[0].Event.ShouldNotBeNull();
      }
   }

   public class When_the_event_presenter_notifies_view_changed : concern_for_SimulationCompoundProtocolEventPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_notify_status_changed()
      {
         _eventRaised.ShouldBeTrue();
      }
   }
}
