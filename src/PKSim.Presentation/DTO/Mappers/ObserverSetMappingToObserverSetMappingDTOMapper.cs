using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IObserverSetMappingToObserverSetMappingDTOMapper
   {
      ObserverSetMappingDTO MapFrom(ObserverSetMapping observerSetMapping, Simulation simulation);
   }

   public class ObserverSetMappingToObserverSetMappingDTOMapper : IObserverSetMappingToObserverSetMappingDTOMapper
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;

      public ObserverSetMappingToObserverSetMappingDTOMapper(IBuildingBlockRepository buildingBlockRepository, IBuildingBlockInSimulationManager buildingBlockInSimulationManager)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
      }

      public ObserverSetMappingDTO MapFrom(ObserverSetMapping observerSetMapping, Simulation simulation)
      {
         var observerSetMappingDTO = new ObserverSetMappingDTO(observerSetMapping);

         var usedTemplateObserverSet = simulation.UsedBuildingBlockByTemplateId(observerSetMapping.TemplateObserverSetId);
         //simulation was already using an event based on template. Retrieved the building block used
         var templateObserverSet = _buildingBlockInSimulationManager.TemplateBuildingBlockUsedBy<ObserverSet>(usedTemplateObserverSet) ??
                                   _buildingBlockRepository.All<ObserverSet>().FindById(observerSetMapping.TemplateObserverSetId);

         observerSetMappingDTO.ObserverSet = templateObserverSet;
         return observerSetMappingDTO;
      }
   }
}