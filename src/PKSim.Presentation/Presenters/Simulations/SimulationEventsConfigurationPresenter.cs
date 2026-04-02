using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationEventsConfigurationPresenter : ISimulationItemPresenter, IParameterValuePresenter
   {
      void AddEventMapping();
      IEnumerable<PKSimEvent> AllEvents();
      void RemoveEventMapping(EventMappingDTO eventMappingDTO);
      Task LoadEventAsync(EventMappingDTO eventMappingDTO);
      void CreateEventFor(EventMappingDTO eventMappingDTO);
   }

   public class SimulationEventsConfigurationPresenter : AbstractSubPresenter<ISimulationEventsConfigurationView, ISimulationEventsConfigurationPresenter>, ISimulationEventsConfigurationPresenter
   {
      private readonly IEditParameterPresenterTask _editParameterPresenterTask;
      private readonly IEventMappingToEventMappingDTOMapper _eventMappingDTOMapper;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IEventTask _eventTask;
      private readonly IEventMappingDTOToEventMappingMapper _eventMappingMapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IEventMappingFactory _eventMappingFactory;
      private readonly INotifyList<EventMappingDTO> _allEventsMappingDTO = new NotifyList<EventMappingDTO>();
      private EventProperties _eventProperties;
      private Simulation _simulation;

      public SimulationEventsConfigurationPresenter(ISimulationEventsConfigurationView view, IEditParameterPresenterTask editParameterPresenterTask,
         IEventMappingToEventMappingDTOMapper eventMappingDTOMapper,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater, IEventTask eventTask,
         IEventMappingDTOToEventMappingMapper eventMappingMapper,
         IBuildingBlockRepository buildingBlockRepository,
         IEventMappingFactory eventMappingFactory)
         : base(view)
      {
         _editParameterPresenterTask = editParameterPresenterTask;
         _eventMappingDTOMapper = eventMappingDTOMapper;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _eventTask = eventTask;
         _eventMappingMapper = eventMappingMapper;
         _buildingBlockRepository = buildingBlockRepository;
         _eventMappingFactory = eventMappingFactory;
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         _simulation = simulation;
         _eventProperties = simulation.EventProperties;
         _allEventsMappingDTO.Clear();
         _eventProperties.EventMappings.Each(addEventMapping);
         _view.BindTo(_allEventsMappingDTO);
      }

      public void AddEventMapping()
      {
         var eventMapping = _eventTask.CreateEventMapping();
         addEventMapping(eventMapping);
         OnStatusChanged();
      }

      private void addEventMapping(EventMapping eventMapping)
      {
         _allEventsMappingDTO.Add(_eventMappingDTOMapper.MapFrom(eventMapping, _simulation));
      }

      public IEnumerable<PKSimEvent> AllEvents() => _eventTask.All();

      public void SaveConfiguration()
      {
         _eventProperties.ClearEventMappings();

         _allEventsMappingDTO.Each(eventMappingDTO =>
         {
            _eventTask.Load(eventMappingDTO.Event);
            _eventProperties.AddEventMapping(_eventMappingMapper.MapFrom(eventMappingDTO, _simulation));
         });

         var allEvents = _allEventsMappingDTO.Select(x => x.Event).ToList();

         addProtocolEventsToEventProperties(allEvents);

         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_simulation, allEvents, PKSimBuildingBlockType.Event);
      }

      private void addProtocolEventsToEventProperties(List<PKSimEvent> allEvents)
      {
         foreach (var compoundProperties in _simulation.CompoundPropertiesList)
         {
            if (!(compoundProperties.ProtocolProperties.Protocol is SimpleProtocol simpleProtocol) || !simpleProtocol.HasEvent)
               continue;

            var pkSimEvent = _buildingBlockRepository.ById<PKSimEvent>(simpleProtocol.TemplateEventId);
            if (pkSimEvent == null)
               continue;

            var eventMapping = _eventMappingFactory.Create(pkSimEvent);
            var offsetValue = simpleProtocol.EventOffsetParameter?.Value ?? 0;
            eventMapping.StartTime.Value = offsetValue;

            _eventTask.Load(pkSimEvent);
            _eventProperties.AddEventMapping(eventMapping);

            if (!allEvents.Contains(pkSimEvent))
               allEvents.Add(pkSimEvent);
         }
      }

      public void RemoveEventMapping(EventMappingDTO eventMappingDTO)
      {
         _allEventsMappingDTO.Remove(eventMappingDTO);
         OnStatusChanged();
      }

      public async Task LoadEventAsync(EventMappingDTO eventMappingDTO)
      {
         var eventTemplate = await _eventTask.SecureAwait(x => x.LoadSingleFromTemplateAsync());
         updateEventInMapping(eventMappingDTO, eventTemplate);
      }

      public void CreateEventFor(EventMappingDTO eventMappingDTO)
      {
         updateEventInMapping(eventMappingDTO, _eventTask.AddToProject());
      }

      private void updateEventInMapping(EventMappingDTO eventMappingDTO, PKSimEvent pkSimEvent)
      {
         if (pkSimEvent == null) return;
         eventMappingDTO.Event = pkSimEvent;
         _view.RefreshData();
         OnStatusChanged();
      }

      public virtual void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         _editParameterPresenterTask.SetParameterPercentile(this, parameterDTO, percentileInPercent);
      }

      public virtual void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         _editParameterPresenterTask.SetParameterValue(this, parameterDTO, valueInGuiUnit);
      }

      public virtual void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit)
      {
         _editParameterPresenterTask.SetParameterUnit(this, parameterDTO, displayUnit);
      }

      public void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         _editParameterPresenterTask.SetParameterValueOrigin(this, parameterDTO, valueOrigin);
      }
   }
}
