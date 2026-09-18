using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
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
      IReadOnlyList<string> UnmappedEventKeys { get; }
   }

   public class SimulationCompoundProtocolEventPresenter : AbstractSubPresenter<ISimulationCompoundProtocolEventView, ISimulationCompoundProtocolEventPresenter>,
      ISimulationCompoundProtocolEventPresenter
   {
      private readonly IEventTask _eventTask;
      private readonly IEventFromMappingRetriever _eventFromMappingRetriever;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private Protocol _protocol;
      private ProtocolProperties _protocolProperties;
      private IEnumerable<EventPlaceholderMappingDTO> _allEventMappingDTO;
      private Simulation _simulation;

      public SimulationCompoundProtocolEventPresenter(
         ISimulationCompoundProtocolEventView view,
         IEventTask eventTask,
         IEventFromMappingRetriever eventFromMappingRetriever,
         IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
         : base(view)
      {
         _eventTask = eventTask;
         _eventFromMappingRetriever = eventFromMappingRetriever;
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
      }

      private IEnumerable<EventPlaceholderMappingDTO> createEventMapping()
      {
         if (_protocol == null)
            return Enumerable.Empty<EventPlaceholderMappingDTO>();

         return (from usedEventKey in _protocol.UsedEventKeys
            where !usedEventKey.IsNullOrEmpty()
            select new EventPlaceholderMappingDTO
            {
               Selection = selectionFrom(eventUsedInSimulationFor(usedEventKey)),
               EventKey = usedEventKey
            }).ToList();
      }

      private PKSimEvent eventUsedInSimulationFor(string eventKey) =>
         _eventFromMappingRetriever.TemplateEventUsedBy(_simulation, _protocolProperties.EventMappingWith(eventKey));

      public IEnumerable<EventSelectionDTO> AllEventsFor(EventPlaceholderMappingDTO eventPlaceholderMappingDTO)
      {
         var hashSet = new HashSet<EventSelectionDTO> { noEventSelection };
         _eventTask.All().Select(selectionFrom).Each(x => hashSet.Add(x));
         if (eventPlaceholderMappingDTO.Selection != null)
            hashSet.Add(eventPlaceholderMappingDTO.Selection);

         return hashSet;
      }

      //a placeholder that is left unmapped applies no event at all
      private static EventSelectionDTO noEventSelection => new EventSelectionDTO { DisplayName = PKSimConstants.UI.NoEvent };

      private EventSelectionDTO selectionFrom(PKSimEvent pkSimEvent) => pkSimEvent == null
         ? noEventSelection
         : new EventSelectionDTO
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

      public IReadOnlyList<string> UnmappedEventKeys =>
         _allEventMappingDTO.Where(x => x.Event == null).Select(x => x.EventKey).ToList();

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

         //an unmapped placeholder applies no event and is simply not saved
         _allEventMappingDTO.Where(dto => dto.Event != null).Each(dto =>
         {
            _eventTask.Load(dto.Event);
            _protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
            {
               TemplateEventId = dto.Event.Id,
               EventKey = dto.EventKey,
               Event = dto.Event
            });
         });
      }
   }
}
