using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Formulations;

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
         var formulationInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, formulation.FormulationType);
         return new FormulationDTO(formulation.AllParameters().ToList())
         {
            Description = formulationInfo.Description,
            Type = new FormulationTypeDTO {Id = formulation.FormulationType, DisplayName = formulationInfo.DisplayName},
         };
      }
   }
}