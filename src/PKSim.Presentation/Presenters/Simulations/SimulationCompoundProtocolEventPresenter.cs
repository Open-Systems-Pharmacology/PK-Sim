using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProtocolEventPresenter : IPresenter<ISimulationCompoundProtocolEventView>, IEditSimulationCompoundPresenter
   {
      IEnumerable<EventSelectionDTO> AllEventsFor(EventPlaceholderMappingDTO eventPlaceholderMappingDTO);
      void CreateEventFor(EventPlaceholderMappingDTO eventPlaceholderMappingDTO);
      Task LoadEventForAsync(EventPlaceholderMappingDTO eventPlaceholderMappingDTO);
      bool EventVisible { get; }
   }

   public class SimulationCompoundProtocolEventPresenter : AbstractSubPresenter<ISimulationCompoundProtocolEventView, ISimulationCompoundProtocolEventPresenter>,
      ISimulationCompoundProtocolEventPresenter
   {
      private readonly IEventTask _eventTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private Protocol _protocol;
      private ProtocolProperties _protocolProperties;
      private IEnumerable<EventPlaceholderMappingDTO> _allEventMappingDTO;
      private Simulation _simulation;

      public SimulationCompoundProtocolEventPresenter(
         ISimulationCompoundProtocolEventView view,
         IEventTask eventTask,
         IBuildingBlockRepository buildingBlockRepository,
         IBuildingBlockInProjectManager buildingBlockInProjectManager,
         IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
         : base(view)
      {
         _eventTask = eventTask;
         _buildingBlockRepository = buildingBlockRepository;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _buildingBlockSelectionDisplayer = buildingBlockSelectionDisplayer;
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         _protocolProperties = compoundProperties.ProtocolProperties;
         _protocol = _protocolProperties.Protocol;
         _simulation = simulation;
         _allEventMappingDTO = createEventMapping().ToList();
         _view.EventVisible = _allEventMappingDTO.Any();
         _view.BindTo(_allEventMappingDTO);
         _view.EventKeyVisible = _allEventMappingDTO.Count() > 1;
      }

      private IEnumerable<EventPlaceholderMappingDTO> createEventMapping()
      {
         if (_protocol == null)
            return Enumerable.Empty<EventPlaceholderMappingDTO>();

         return (from usedEventKey in _protocol.UsedEventKeys
            where !usedEventKey.IsNullOrEmpty()
            let pkSimEvent = eventUsedInSimulationFor(usedEventKey) ?? defaultEvent()
            select new EventPlaceholderMappingDTO
            {
               Selection = selectionFrom(pkSimEvent),
               EventKey = usedEventKey
            }).ToList();
      }

      private PKSimEvent defaultEvent() => _eventTask.All().FirstOrDefault();

      private PKSimEvent eventUsedInSimulationFor(string eventKey)
      {
         var mapping = _protocolProperties.EventMappingWith(eventKey);
         if (mapping == null || mapping.TemplateEventId.IsNullOrEmpty())
            return null;

         var templateEvent = _buildingBlockRepository.ById<PKSimEvent>(mapping.TemplateEventId);
         var usedBuildingBlock = _simulation.UsedBuildingBlockByTemplateId(mapping.TemplateEventId);

         if (usedBuildingBlock == null)
            return templateEvent;

         return _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<PKSimEvent>(usedBuildingBlock) ?? templateEvent;
      }

      public IEnumerable<EventSelectionDTO> AllEventsFor(EventPlaceholderMappingDTO eventPlaceholderMappingDTO)
      {
         var eventSelections = _eventTask.All().Select(selectionFrom);
         var hashSet = new HashSet<EventSelectionDTO>(eventSelections);
         if (eventPlaceholderMappingDTO.Selection != null)
            hashSet.Add(eventPlaceholderMappingDTO.Selection);

         return hashSet;
      }

      private EventSelectionDTO selectionFrom(PKSimEvent pkSimEvent) => new EventSelectionDTO
      {
         BuildingBlock = pkSimEvent,
         DisplayName = _buildingBlockSelectionDisplayer.DisplayNameFor(pkSimEvent)
      };

      public void CreateEventFor(EventPlaceholderMappingDTO eventPlaceholderMappingDTO)
      {
         var pkSimEvent = _eventTask.AddToProject();
         if (pkSimEvent == null) return;
         updateEventInMapping(eventPlaceholderMappingDTO, pkSimEvent);
      }

      public async Task LoadEventForAsync(EventPlaceholderMappingDTO eventPlaceholderMappingDTO)
      {
         var pkSimEvent = await _eventTask.SecureAwait(x => x.LoadSingleFromTemplateAsync());
         updateEventInMapping(eventPlaceholderMappingDTO, pkSimEvent);
      }

      public bool EventVisible => _view.EventVisible;

      private void updateEventInMapping(EventPlaceholderMappingDTO eventPlaceholderMappingDTO, PKSimEvent pkSimEvent)
      {
         if (pkSimEvent == null) return;
         eventPlaceholderMappingDTO.Selection = selectionFrom(pkSimEvent);
         _view.RefreshData();
         OnStatusChanged();
      }

      public void SaveConfiguration()
      {
         _protocolProperties.ClearEventPlaceholderMappings();

         _allEventMappingDTO.Each(dto =>
         {
            _eventTask.Load(dto.Event);
            _protocolProperties.AddEventPlaceholderMapping(mapFrom(dto));
         });
      }

      private EventPlaceholderMapping mapFrom(EventPlaceholderMappingDTO dto)
      {
         var templateEventId = dto.Event.Id;
         var templateEvent = _buildingBlockRepository.ById<PKSimEvent>(templateEventId);

         if (templateEvent == null)
         {
            var usedEvent = _simulation.UsedBuildingBlockById(templateEventId);
            templateEventId = usedEvent.TemplateId;
         }

         return new EventPlaceholderMapping
         {
            TemplateEventId = templateEventId,
            EventKey = dto.EventKey,
            Event = dto.Event
         };
      }
   }
}
