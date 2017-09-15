using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Parameters;

using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationEventsConfigurationPresenter : ISimulationItemPresenter, IEditParameterPresenter
   {
      void AddEventMapping();
      IEnumerable<PKSimEvent> AllEvents();
      void RemoveEventMapping(EventMappingDTO eventMappingDTO);
      void LoadEventFor(EventMappingDTO eventMappingDTO);
      void CreateEventFor(EventMappingDTO eventMappingDTO);
   }

   public class SimulationEventsConfigurationPresenter : AbstractSubPresenter<ISimulationEventsConfigurationView, ISimulationEventsConfigurationPresenter>, ISimulationEventsConfigurationPresenter
   {
      private readonly IEditParameterPresenterTask _editParameterPresenterTask;
      private readonly IEventMappingToEventMappingDTOMapper _eventMappingDTOMapper;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IEventTask _eventTask;
      private readonly IEventMappingDTOToEventMappingMapper _eventMappingMapper;
      private readonly INotifyList<EventMappingDTO> _allEventsMappingDTO = new NotifyList<EventMappingDTO>();
      private EventProperties _eventProperties;
      private Simulation _simulation;

      public SimulationEventsConfigurationPresenter(ISimulationEventsConfigurationView view, IEditParameterPresenterTask editParameterPresenterTask,
         IEventMappingToEventMappingDTOMapper eventMappingDTOMapper,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater, IEventTask eventTask,
         IEventMappingDTOToEventMappingMapper eventMappingMapper)
         : base(view)
      {
         _editParameterPresenterTask = editParameterPresenterTask;
         _eventMappingDTOMapper = eventMappingDTOMapper;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _eventTask = eventTask;
         _eventMappingMapper = eventMappingMapper;
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
         _allEventsMappingDTO.Add(_eventMappingDTOMapper.MapFrom(eventMapping, _simulation));
         OnStatusChanged();
      }

      private void addEventMapping(EventMapping eventMapping)
      {
         _allEventsMappingDTO.Add(_eventMappingDTOMapper.MapFrom(eventMapping, _simulation));
      }

      public IEnumerable<PKSimEvent> AllEvents()
      {
         return _eventTask.All();
      }

      public void SaveConfiguration()
      {
         _eventProperties.ClearEventMapping();

         _allEventsMappingDTO.Each(eventMappingDTO =>
         {
            _eventTask.Load(eventMappingDTO.Event);
            _eventProperties.AddEventMapping(_eventMappingMapper.MapFrom(eventMappingDTO, _simulation));
         });

         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_simulation, _allEventsMappingDTO.Select(x => x.Event), PKSimBuildingBlockType.Event);
      }

      public void RemoveEventMapping(EventMappingDTO eventMappingDTO)
      {
         _allEventsMappingDTO.Remove(eventMappingDTO);
         OnStatusChanged();
      }

      public void LoadEventFor(EventMappingDTO eventMappingDTO)
      {
         updateEventInMapping(eventMappingDTO, _eventTask.LoadSingleFromTemplate());
      }

      public void CreateEventFor(EventMappingDTO eventMappingDTO)
      {
         updateEventInMapping(eventMappingDTO, _eventTask.CreateEvent());
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

      public void SetParameterValueDescription(IParameterDTO parameterDTO, string valueDescription)
      {
         _editParameterPresenterTask.SetParameterValueDescription(this, parameterDTO, valueDescription);
      }

   }
}