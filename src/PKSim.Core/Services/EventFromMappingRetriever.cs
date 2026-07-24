using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IEventFromMappingRetriever
   {
      /// <summary>
      ///    Returns the template event building block used in the simulation for the given mapping.
      ///    If the status of the building block is changed, returns the used building block in the simulation, otherwise the
      ///    template building block
      /// </summary>
      PKSimEvent TemplateEventUsedBy(Simulation simulation, EventPlaceholderMapping eventPlaceholderMapping);
   }

   public class EventFromMappingRetriever : IEventFromMappingRetriever
   {
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public EventFromMappingRetriever(IBuildingBlockInProjectManager buildingBlockInProjectManager, IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public PKSimEvent TemplateEventUsedBy(Simulation simulation, EventPlaceholderMapping eventPlaceholderMapping)
      {
         if (eventPlaceholderMapping == null || eventPlaceholderMapping.TemplateEventId.IsNullOrEmpty())
            return null;

         var templateEvent = _buildingBlockRepository.ById<PKSimEvent>(eventPlaceholderMapping.TemplateEventId);
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(eventPlaceholderMapping.TemplateEventId);

         if (usedBuildingBlock == null)
            return templateEvent;

         return _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<PKSimEvent>(usedBuildingBlock) ?? templateEvent;
      }
   }
}
