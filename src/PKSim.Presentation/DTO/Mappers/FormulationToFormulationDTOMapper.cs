using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Formulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IFormulationToFormulationDTOMapper : IMapper<Formulation, FormulationDTO>
   {
   }

   public class FormulationToFormulationDTOMapper : IFormulationToFormulationDTOMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public FormulationToFormulationDTOMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public FormulationDTO MapFrom(Formulation formulation)
      {
         var formulationDTO = new FormulationDTO();
         var formulationInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, formulation.FormulationType);
         formulationDTO.Description = formulationInfo.Description;
         formulationDTO.Type = new FormulationTypeDTO {Id = formulation.FormulationType, DisplayName = formulationInfo.DisplayName};

         formulationDTO.Parameters = formulation.AllParameters().ToList();
         return formulationDTO;
      }
   }
}