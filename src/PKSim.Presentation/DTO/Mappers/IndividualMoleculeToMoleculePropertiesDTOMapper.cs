using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualMoleculeToMoleculePropertiesDTOMapper : IMapper<IndividualMolecule, MoleculePropertiesDTO>
   {
   }

   public class IndividualMoleculeToMoleculePropertiesDTOMapper : IIndividualMoleculeToMoleculePropertiesDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<MoleculePropertiesDTO> _parameterMapper;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public IndividualMoleculeToMoleculePropertiesDTOMapper(IParameterToParameterDTOInContainerMapper<MoleculePropertiesDTO> parameterMapper, IObjectTypeResolver objectTypeResolver)
      {
         _parameterMapper = parameterMapper;
         _objectTypeResolver = objectTypeResolver;
      }

      public MoleculePropertiesDTO MapFrom(IndividualMolecule molecule)
      {
         var dto = new MoleculePropertiesDTO(molecule) {MoleculeType = _objectTypeResolver.TypeFor(molecule)};
         dto.ReferenceConcentrationParameter = _parameterMapper.MapFrom(molecule.ReferenceConcentration, dto, x => x.ReferenceConcentration, x => x.ReferenceConcentrationParameter);
         dto.HalfLifeLiverParameter = _parameterMapper.MapFrom(molecule.HalfLifeLiver, dto, x => x.HalfLifeLiver, x => x.HalfLifeLiverParameter);
         dto.HalfLifeIntestineParameter = _parameterMapper.MapFrom(molecule.HalfLifeIntestine, dto, x => x.HalfLifeIntestine, x => x.HalfLifeIntestineParameter);
         return dto;
      }
   }
}