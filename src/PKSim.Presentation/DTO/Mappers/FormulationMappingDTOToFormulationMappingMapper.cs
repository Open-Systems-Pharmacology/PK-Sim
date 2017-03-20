using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IFormulationMappingDTOToFormulationMappingMapper
   {
      FormulationMapping MapFrom(FormulationMappingDTO formulationMappingDTO, Simulation simulation);
   }

   public class FormulationMappingDTOToFormulationMappingMapper : IFormulationMappingDTOToFormulationMappingMapper
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public FormulationMappingDTOToFormulationMappingMapper(IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public FormulationMapping MapFrom(FormulationMappingDTO formulationMappingDTO, Simulation simulation)
      {
         var templateFormulationId = formulationMappingDTO.Formulation.Id;
         var templateFormulation = _buildingBlockRepository.All().FindById(templateFormulationId);

         //in that case, formulation is a simulation formulation, we need to retrieve the template formulation
         if (templateFormulation == null)
         {
            var usedFormulation = simulation.UsedBuildingBlockById(templateFormulationId);
            templateFormulationId = usedFormulation.TemplateId;
         }

         return new FormulationMapping
         {
            TemplateFormulationId = templateFormulationId,
            FormulationKey = formulationMappingDTO.FormulationKey,
            Formulation = formulationMappingDTO.Formulation
         };
      }
   }
}