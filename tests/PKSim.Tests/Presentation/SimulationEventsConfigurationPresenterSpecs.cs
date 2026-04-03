using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationEventsConfigurationPresenter : ContextSpecification<ISimulationEventsConfigurationPresenter>
   {
      protected ISimulationEventsConfigurationView _view;
      private IEditParameterPresenterTask _editParameterPresenterTask;
      protected IEventMappingToEventMappingDTOMapper _eventMappingDTOMapper;
      private ISimulationBuildingBlockUpdater _simBuildingBlockUpdater;
      private IEventTask _eventTask;
      private IEventMappingDTOToEventMappingMapper _eventMappingMapper;

      protected override void Context()
      {
         _view = A.Fake<ISimulationEventsConfigurationView>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _eventMappingDTOMapper = A.Fake<IEventMappingToEventMappingDTOMapper>();
         _simBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _eventTask = A.Fake<IEventTask>();
         _eventMappingMapper = A.Fake<IEventMappingDTOToEventMappingMapper>();
         sut = new SimulationEventsConfigurationPresenter(_view, _editParameterPresenterTask, _eventMappingDTOMapper, _simBuildingBlockUpdater, _eventTask, _eventMappingMapper);
      }
   }

   public class When_calling_the_edit_simulation_twice : concern_for_SimulationEventsConfigurationPresenter
   {
      private Simulation _simulation;
      private EventProperties _eventProperties;
      private EventMapping _event1;
      private EventMappingDTO _eventDTO;
      private IEnumerable<EventMappingDTO> _eventsDTO;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _eventProperties = new EventProperties();
         _event1 = new EventMapping();
         _eventDTO = new EventMappingDTO(_event1);
         _eventProperties.AddEventMapping(_event1);
         A.CallTo(() => _eventMappingDTOMapper.MapFrom(_event1, _simulation)).Returns(_eventDTO);
         A.CallTo(() => _simulation.EventProperties).Returns(_eventProperties);
         A.CallTo(() => _view.BindTo(A<IEnumerable<EventMappingDTO>>._))
            .Invokes(x => _eventsDTO = x.GetArgument<IEnumerable<EventMappingDTO>>(0));
      }

      protected override void Because()
      {
         //first time
         sut.EditSimulation(_simulation, CreationMode.New);

         //second time
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_not_add_the_events_of_the_former_simulation()
      {
         _eventsDTO.Count().ShouldBeEqualTo(1);
      }
   }
}